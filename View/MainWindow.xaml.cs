using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Graphics;
using Graphics.Primitives;
using Color = System.Drawing.Color;
using System.Configuration;

namespace View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Canvas _canvas;

        public MainWindow()
        {
            InitializeComponent();

            _canvas = CreateCanvas();

            //DrawLines();
            DrawObj();
            //DrawTriangle();
            
            CanvasView.Source = ConvertCanvasToSource(_canvas);
        }

        private Canvas CreateCanvas()
        {
            int width = (int)Width, height = (int)Height;
            var canvas = new Canvas(width, height, Color.Black);
            canvas.SetColor(Color.White);

            return canvas;
        }

        private void DrawTriangle()
        {
            var points = new Vector2Int[]
            {
                new Vector2Int(300, 280),
                new Vector2Int(840, 240),
                new Vector2Int(570, 150),
            };

            _canvas.SetColor(Color.Aqua);
            _canvas.DrawTriangle(points, Line.LineType.Wu);
            _canvas.FillTriangle(points);
        }

        private void DrawObj()
        {
            string texPath = ConfigurationManager.AppSettings["TexPath"];
            string path = ConfigurationManager.AppSettings["ObjPath"];

            StringBuilder builder = new StringBuilder();            
            Stopwatch watch = Stopwatch.StartNew();
            Mesh mesh = ObjParser.GetMeshFromFile(path);            
            watch.Stop();
            builder.AppendLine($"Parsing: {watch.ElapsedMilliseconds} ms");
            watch = Stopwatch.StartNew();
            mesh.Texture = Bitmap.FromFile(texPath) as Bitmap;            
            watch.Stop();
            builder.AppendLine($"Loading texture: {watch.ElapsedMilliseconds} ms");
            watch = Stopwatch.StartNew();
            _canvas.DrawMesh(mesh, Line.LineType.Bresenham);            
            watch.Stop();
            builder.AppendLine($"Drawing: {watch.ElapsedMilliseconds} ms");
            LogView.Text = builder.ToString();
        }

        private void DrawLines()
        {
            int width = (int)Width, height = (int)Height;
            Vector2Int center = new Vector2Int(width / 2, height / 2);

            for (int i = 1; i < 18; i++)
            {
                int x = (int)(width / 2 + 200 * Math.Cos(i * 2 * 3.14 / 17));
                int y = (int)(height / 2 + 200 * Math.Sin(i * 2 * 3.14 / 17));
                _canvas.DrawLine(center, new Vector2Int(x, y), Line.LineType.Wu);
            }
        }

        private BitmapImage ConvertCanvasToSource(Canvas canvas)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                canvas.Bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
