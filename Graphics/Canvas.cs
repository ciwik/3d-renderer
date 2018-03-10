using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Graphics.Primitives;
using Microsoft.Win32.SafeHandles;

namespace Graphics
{
    public class Canvas
    {
        private BitmapLock _bitmapLock;
        private Color _color = Color.Black;
        private int _width, _height;
        private float[,] _zBuffer;

        private BitmapLock _texture;

        public Bitmap Bitmap => _bitmapLock.Release();

        private Canvas()
        {
        }

        private void FillBitmap(Color color)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    DrawPoint(x, y, color);
                }
            }
        }

        public Canvas(int width, int height, Color color)
        {
            _width = width;
            _height = height;
            Bitmap bitmap = new Bitmap(_width, _height);
            _bitmapLock = new BitmapLock(bitmap, ImageLockMode.ReadWrite);

            FillBitmap(color);

            _zBuffer = new float[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _zBuffer[x, y] = float.MinValue;
                }
            }
        }

        public void SetColor(Color color)
        {
            _color = color;
        }
        
        public void DrawPoint(int x, int y, Color color)
        {
            _bitmapLock.SetPixel(x, y, color);
        }

        #region Line

        private float step = 0.01f;

        private void DrawLineDummy(Vector2Int from, Vector2Int to)
        {
            for (float t = 0; t <= 1f; t += step)
            {
                var point = Vector2Int.Lerp(from, to, t);
                DrawPoint(point.X, point.Y, _color);
            }
        }

        private void DrawLineBresenham(Vector2Int from, Vector2Int to)
        {
            Vector2Int diff = to - from;
            bool steep = Math.Abs(diff.Y) > Math.Abs(diff.X);
            if (steep)
            {
                from = new Vector2Int(from.Y, from.X);
                to = new Vector2Int(to.Y, to.X);
            }
            diff = to - from;
            if (diff.X < 0)
            {
                Vector2Int.Swap(ref from, ref to);
            }

            diff.Y = Math.Abs(diff.Y);

            int dx = to.X - from.X, dy = to.Y - from.Y;
            float error = 0, dError = Math.Abs(dy / (float) dx);
            int yDir = from.Y < to.Y ? 1 : -1;

            int y = from.Y;
            for (int x = from.X; x <= to.X; x++)
            {
                if (steep)
                    DrawPoint(y, x, _color);
                else
                    DrawPoint(x, y, _color);
                error += dError;
                if (error > 0.5f)
                {
                    y += yDir;
                    error -= 1;
                }
            }
        }

        private void DrawLineWu(Vector2Int from, Vector2Int to)
        {
            Vector2Int diff = to - from;
            bool steep = Math.Abs(diff.Y) > Math.Abs(diff.X);
            if (steep)
            {
                from = new Vector2Int(from.Y, from.X);
                to = new Vector2Int(to.Y, to.X);
            }
            diff = to - from;
            if (diff.X < 0)
            {
                Vector2Int.Swap(ref from, ref to);
            }

            float dx = to.X - from.X, dy = to.Y - from.Y;
            float gradient = dy / dx;
            float y = from.Y;
            for (int x = from.X; x <= to.X; x++)
            {
                var color = Color.FromArgb((int) (_color.A * (1 - (y - (int) y))), _color);
                if (steep)
                    DrawPoint((int) y, x, color);
                else
                    DrawPoint(x, (int) y, color);

                color = Color.FromArgb((int) (_color.A * (y - (int) y)), _color);
                if (steep)
                    DrawPoint((int) y + 1, x, color);
                else
                    DrawPoint(x, (int) y + 1, color);

                y += gradient;
            }
        }

        public void DrawLine(Line line)
        {
            switch (line.Type)
            {
                case Line.LineType.Dummy:
                    DrawLineDummy(line.From, line.To);
                    break;
                case Line.LineType.Bresenham:
                    DrawLineBresenham(line.From, line.To);
                    break;
                case Line.LineType.Wu:
                    DrawLineWu(line.From, line.To);
                    break;
                default:
                    DrawLineDummy(line.From, line.To);
                    break;
            }
        }

        public void DrawLine(Vector2Int @from, Vector2Int to, Line.LineType type)
        {
            DrawLine(new Line(@from, to, type));
        }

        #endregion

        public void DrawPolygon(Polygon polygon, float intensity, Line.LineType lineType)
        {
            byte gray = (byte)(intensity * 255);
            SetColor(Color.FromArgb(255, gray, gray, gray));
            Vector2Int[] points = new Vector2Int[3];
            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                points[i] = GetScreenPoint(polygon.Vertices[i]);
            }
            
            //DrawTriangle(points, lineType);
            FillTriangle(points, polygon);            
            //FillTriangle(points);
            //FillTriangle2(points);
        }

        public void DrawPolygonWithTex(Polygon polygon, Line.LineType lineType)
        {
            Vector2Int[] points = new Vector2Int[3];
            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                points[i] = GetScreenPoint(polygon.Vertices[i]);
            }

            FillTriangleWithTex(points, polygon);
        }

        public void DrawTriangle(Vector2Int[] points, Line.LineType lineType)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector2Int v1 = points[i];
                Vector2Int v2 = points[(i+1) % 3];
                DrawLine(v1, v2, lineType);
            }
        }

        public void FillTriangle(Vector2Int[] points)
        {
            Vector2Int min = new Vector2Int(), max = new Vector2Int();

            min.X = points.Min(p => p.X);
            min.Y = points.Min(p => p.Y);
            max.X = points.Max(p => p.X);
            max.Y = points.Max(p => p.Y);

            Parallel.For(min.X, max.X + 1, x =>
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    Vector3 bar = GetBarycentricCoords(new Vector2Int(x, y), points);
                    if (bar.X > 0 && bar.Y > 0 && bar.Z > 0)
                        DrawPoint(x, y, _color);
                }
            });
            /*for (int x = min.X; x <= max.X; x++)
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    Vector3 bar = GetBarycentricCoords(new Vector2Int(x, y), points);
                    if (bar.X > 0 && bar.Y > 0 && bar.Z > 0)
                        DrawPoint(x, y, _color);
                }
            }*/
        }

        public void FillTriangle(Vector2Int[] points, Polygon polygon)  //with z-buffer
        {
            Vector2Int min = new Vector2Int(), max = new Vector2Int();

            min.X = points.Min(p => p.X);
            min.Y = points.Min(p => p.Y);
            max.X = points.Max(p => p.X);
            max.Y = points.Max(p => p.Y);

            Parallel.For(min.X - 1, max.X + 1, x =>
            {
                for (int y = min.Y - 1; y <= max.Y + 1; y++)
                {
                    Vector3 bar = GetBarycentricCoords(new Vector2Int(x, y), points);
                    if (bar.X >= -2 * Single.Epsilon && bar.Y >= -2 * Single.Epsilon && bar.Z >= -2 * Single.Epsilon)
                    {
                        float zCoord = polygon.Vertices[0].Z * bar.X + polygon.Vertices[1].Z * bar.Y +
                                       polygon.Vertices[2].Z * bar.Z;
                        if (_zBuffer[x, y] < zCoord)
                        {
                            DrawPoint(x, y, _color);
                            _zBuffer[x, y] = zCoord;
                        }
                    }
                }
            });
        }

        public void FillTriangleWithTex(Vector2Int[] points, Polygon polygon)  //with texture
        {
            Vector2Int min = new Vector2Int(), max = new Vector2Int();

            min.X = points.Min(p => p.X);
            min.Y = points.Min(p => p.Y);
            max.X = points.Max(p => p.X);
            max.Y = points.Max(p => p.Y);

            Parallel.For(min.X - 1, max.X + 1, x =>
            {
                for (int y = min.Y - 1; y <= max.Y + 1; y++)
                {
                    Vector3 bar = GetBarycentricCoords(new Vector2Int(x, y), points);
                    if (bar.X >= -2*Single.Epsilon && bar.Y >= -2*Single.Epsilon && bar.Z >= -2*Single.Epsilon)
                    {
                        float zCoord = polygon.Vertices[0].Z * bar.X + polygon.Vertices[1].Z * bar.Y +
                                       polygon.Vertices[2].Z * bar.Z;
                        if (_zBuffer[x, y] < zCoord)
                        {
                            Vector2 uvCoords = new Vector2(
                                polygon.UVs[0].X * bar.X + polygon.UVs[1].X * bar.Y + polygon.UVs[2].X * bar.Z,
                                polygon.UVs[0].Y * bar.X + polygon.UVs[1].Y * bar.Y + polygon.UVs[2].Y * bar.Z);
                            Vector2Int texCoords = new Vector2Int((int) (_texture.Width * uvCoords.X),
                                (int) (_texture.Height * (1 - uvCoords.Y)));
                            Color color = _texture.GetPixel(texCoords.X, texCoords.Y);
                            DrawPoint(x, y, color);
                            _zBuffer[x, y] = zCoord;
                        }                        
                    }
                }
            });
        }

        private Vector3 GetBarycentricCoords(Vector2Int pos, Vector2Int[] triangle)
        {
            Vector3 result = new Vector3();
            result.X = ((pos.Y - triangle[2].Y)*(triangle[1].X - triangle[2].X) - (pos.X - triangle[2].X)*(triangle[1].Y - triangle[2].Y))/
                (float)((triangle[0].Y - triangle[2].Y) * (triangle[1].X - triangle[2].X) - (triangle[0].X - triangle[2].X) * (triangle[1].Y - triangle[2].Y));

            result.Y = ((pos.Y - triangle[0].Y) * (triangle[2].X - triangle[0].X) - (pos.X - triangle[0].X) * (triangle[2].Y - triangle[0].Y)) /
                       (float)((triangle[1].Y - triangle[0].Y) * (triangle[2].X - triangle[0].X) - (triangle[1].X - triangle[0].X) * (triangle[2].Y - triangle[0].Y));


            result.Z = 1 - result.X - result.Y;
            //result.Z = ((pos.Y - triangle[1].Y) * (triangle[0].X - triangle[1].X) - (pos.X - triangle[1].X) * (triangle[0].Y - triangle[1].Y)) /
            //           (float)((triangle[2].Y - triangle[1].Y) * (triangle[0].X - triangle[1].X) - (triangle[2].X - triangle[1].X) * (triangle[0].Y - triangle[1].Y));
            return result;
        }

        public void FillTriangle2(Vector2Int[] points)
        {
            points = points.OrderBy(p => p.X).ToArray();
            Vector2Int A = points[0], B = points[1], C = points[2];

            Parallel.For(A.X + 1, B.X + 1, x =>
            {
                float t = (x - A.X) / (float)(B.X - A.X);
                int Y1 = Vector2Int.Lerp(A, B, t).Y;
                t = (x - A.X) / (float)(C.X - A.X);
                int Y2 = Vector2Int.Lerp(A, C, t).Y;

                if (Y1 > Y2)
                {
                    Swap(ref Y1, ref Y2);
                }
                for (int y = Y1; y <= Y2; y++)
                {
                    DrawPoint(x, y, _color);
                }
            });
            /*for (int x = A.X + 1; x <= B.X; x++)
            {
                float t = (x - A.X) / (float)(B.X - A.X);
                int Y1 = Vector2Int.Lerp(A, B, t).Y;
                t = (x - A.X) / (float)(C.X - A.X);
                int Y2 = Vector2Int.Lerp(A, C, t).Y;

                if (Y1 > Y2)
                {
                    Swap(ref Y1, ref Y2);
                }
                for (int y = Y1; y <= Y2; y++)
                {
                    DrawPoint(x, y, _color);
                }
            }*/

            Parallel.For(B.X + 1, C.X + 1, x => {
                float t = (x - B.X) / (float)(C.X - B.X);
                int Y1 = Vector2Int.Lerp(B, C, t).Y;
                t = (x - A.X) / (float)(C.X - A.X);
                int Y2 = Vector2Int.Lerp(A, C, t).Y;

                if (Y1 > Y2)
                {
                    Swap(ref Y1, ref Y2);
                }

                for (int y = Y1; y <= Y2; y++)
                {
                    DrawPoint(x, y, _color);
                }
            });
            /*for (int x = B.X + 1; x <= C.X; x++)
            {
                float t = (x - B.X) / (float)(C.X - B.X);
                int Y1 = Vector2Int.Lerp(B, C, t).Y;
                t = (x - A.X) / (float)(C.X - A.X);
                int Y2 = Vector2Int.Lerp(A, C, t).Y;

                if (Y1 > Y2)
                {
                    Swap(ref Y1, ref Y2);
                }

                for (int y = Y1; y <= Y2; y++)
                {
                    DrawPoint(x, y, _color);
                }
            }*/
        }

        public void DrawMesh(Mesh mesh, Line.LineType lineType)
        {
            _texture = new BitmapLock(mesh.Texture, ImageLockMode.ReadOnly);

            foreach (Polygon polygon in mesh.Polygons)
            {                
                float intensity = 0f;
                if (IsPolygonShouldBeDrawn(polygon, out intensity))
                {
                    if (polygon.UVs.Length == 0)
                        DrawPolygon(polygon, intensity, lineType);
                    else
                        DrawPolygonWithTex(polygon, lineType);
                }
            }
        }

        private Vector3 _lightDirection = new Vector3(0, 0, 1);

        private Color GetRandomColor()
        {
            byte r = (byte) new Random(5 + DateTime.Now.Millisecond).Next(0, 256);
            byte g = (byte)new Random(r + DateTime.Now.Millisecond + DateTime.Now.Minute).Next(0, 256);
            byte b = (byte)new Random(r + g + DateTime.Now.Millisecond).Next(0, 256);
            r = (byte)new Random(r + g + b + DateTime.Now.Millisecond).Next(0, 256);
            return Color.FromArgb(255, r, g, b);            
        }

        //back-face culling
        private bool IsPolygonShouldBeDrawn(Polygon polygon, out float intensity)
        {
            Vector2Int[] triangle = new Vector2Int[3];            
            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                triangle[i] = GetScreenPoint(polygon.Vertices[i]);
            }
            Vector3 normal = GetNormal(polygon);
            intensity = Vector3.Dot(normal, _lightDirection);
            return intensity > 0;
        }

        private Vector2Int GetScreenPoint(Vector3 v)
        {            
            return new Vector2Int(Convert.ToInt32((1 + v.X) * (_width - 1) / 2),
                Convert.ToInt32((1 - v.Y) * (_height - 1) / 2));
        }

        private Vector3 GetNormal(Polygon polygon)
        {
            Vector3 v1 = polygon.Vertices[1] - polygon.Vertices[0];
            Vector3 v2 = polygon.Vertices[2] - polygon.Vertices[0];

            return Vector3.Cross(v1, v2).Normalized;
        }

        private void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
}
