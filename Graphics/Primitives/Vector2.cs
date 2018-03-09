namespace Graphics.Primitives
{
    public struct Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator *(Vector2 v, float a)
        {
            return new Vector2(v.X * a, v.Y * a);
        }

        public static Vector2 operator /(Vector2 v, float a)
        {
            return new Vector2(v.X / a, v.Y / a);
        }

        public static Vector2 Lerp(Vector2 v1, Vector2 v2, float t)
        {
            float x = v1.X + (v2.X - v1.X) * t;
            float y = v1.Y + (v2.Y - v1.Y) * t;

            return new Vector2(x, y);
        }

        public override string ToString()
        {
            return $"{X}; {Y}";
        }
    }
}
