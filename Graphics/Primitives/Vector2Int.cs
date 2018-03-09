namespace Graphics.Primitives
{
    public struct Vector2Int
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector2Int operator +(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2Int operator -(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2Int operator *(Vector2Int v, float a)
        {
            return new Vector2Int((int)(v.X * a), (int)(v.Y * a));
        }

        public static Vector2Int operator /(Vector2Int v, float a)
        {
            return new Vector2Int((int)(v.X / a), (int)(v.Y / a));
        }

        public static Vector2Int Lerp(Vector2Int v1, Vector2Int v2, float t)
        {
            int x = (int)(v1.X + (v2.X - v1.X) * t);
            int y = (int)(v1.Y + (v2.Y - v1.Y) * t);

            return new Vector2Int(x, y);
        }

        public static void Swap(ref Vector2Int a, ref Vector2Int b)
        {
            Vector2Int c = a;
            a = b;
            b = c;
        }

        public override string ToString()
        {
            return $"{X}; {Y}";
        }
    }
}
