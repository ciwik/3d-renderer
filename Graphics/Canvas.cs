using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Graphics.Primitives;

namespace Graphics
{
    public class Canvas
    {
        private Bitmap _bitmap;
        private Color _color = Color.Black;

        public Bitmap Bitmap => _bitmap;

        private Canvas()
        {
        }

        private void FillBitmap(Color color)
        {
            for (int x = 0; x < _bitmap.Width; x++)
            {
                for (int y = 0; y < _bitmap.Height; y++)
                {
                    _bitmap.SetPixel(x, y, color);
                }
            }
        }

        public Canvas(int width, int height, Color color)
        {
            _bitmap = new Bitmap(width, height);
            FillBitmap(color);
        }

        public void SetColor(Color color)
        {
            _color = color;
        }

        public static Canvas FromFile(string filePath)
        {
            var canvas = new Canvas();
            canvas._bitmap = Image.FromFile(filePath) as Bitmap;
            return canvas;
        }

        public static void SaveToFile(Canvas canvas, string filePath)
        {
            canvas._bitmap.Save(filePath);
        }

        #region Line

        private float step = 0.01f;

        private void DrawLineDummy(Vector2Int from, Vector2Int to)
        {
            for (float t = 0; t <= 1f; t += step)
            {
                var point = Vector2Int.Lerp(from, to, t);
                _bitmap.SetPixel(point.X, point.Y, _color);
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
                    _bitmap.SetPixel(y, x, _color);
                else
                    _bitmap.SetPixel(x, y, _color);
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
                var color = Color.FromArgb((int) (_color.A * (1 - (y - (int) y))), _color.R, _color.G, _color.B);
                if (steep)
                    _bitmap.SetPixel((int) y, x, color);
                else
                    _bitmap.SetPixel(x, (int) y, color);

                color = Color.FromArgb((int) (_color.A * (y - (int) y)), _color.R, _color.G, _color.B);
                if (steep)
                    _bitmap.SetPixel((int) y + 1, x, color);
                else
                    _bitmap.SetPixel(x, (int) y + 1, color);

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
            DrawTriangle(points, lineType);
            FillTriangle(points);
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
            points = points.OrderBy(p => p.X).ToArray();
            Vector2Int A = points[0], B = points[1], C = points[2];

            for (int x = A.X + 1; x <= B.X; x++)
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
                    _bitmap.SetPixel(x, y, _color);
                }
            }

            for (int x = B.X + 1; x <= C.X; x++)
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
                    _bitmap.SetPixel(x, y, _color);
                }
            }
        }

        public void DrawMesh(Mesh mesh, Line.LineType lineType)
        {
            foreach (Polygon polygon in mesh.Polygons)
            {                
                //SetColor(GetRandomColor());
                float intensity = 0f;
                if (IsPolygonShouldBeDrawn(polygon, ref intensity))
                    DrawPolygon(polygon, intensity, lineType);
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
        private bool IsPolygonShouldBeDrawn(Polygon polygon, ref float intensity)
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
            return new Vector2Int((int)((1 + v.X) * _bitmap.Width / 2.1f),
                (int)((1 - v.Y) * _bitmap.Height / 2.1f));
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
