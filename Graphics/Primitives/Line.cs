using System.Drawing;

namespace Graphics.Primitives
{
    public class Line
    {
        public enum LineType
        {
            Dummy = 0,
            Bresenham = 1,
            Wu = 2
        }

        public LineType Type { get; }
        public Vector2Int From { get; }
        public Vector2Int To { get; }

        public Line(Vector2Int @from, Vector2Int to, LineType type)
        {
            From = @from;
            To = to;
            Type = type;
        }

        internal void Draw(Bitmap bitmap)
        {
            
        }
    }
}
