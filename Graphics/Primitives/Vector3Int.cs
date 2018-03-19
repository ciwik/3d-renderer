namespace Graphics.Primitives
{
    public struct Vector3Int
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3Int(Vector2Int v)
        {
            X = v.X;
            Y = v.Y;
            Z = 0;
        }

        public static Vector3Int operator +(Vector3Int v1, Vector3Int v2)
        {
            return new Vector3Int(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3Int operator -(Vector3Int v1, Vector3Int v2)
        {
            return new Vector3Int(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3Int operator *(Vector3Int v, float a)
        {
            return new Vector3Int((int)(v.X * a), (int)(v.Y * a), (int)(v.Z * a));
        }

        public static Vector3Int operator /(Vector3Int v, float a)
        {
            return new Vector3Int((int)(v.X / a), (int)(v.Y / a), (int)(v.Z / a));
        }

        public static Vector3Int Lerp(Vector3Int v1, Vector3Int v2, float t)
        {
            int x = (int)(v1.X + (v2.X - v1.X) * t);
            int y = (int)(v1.Y + (v2.Y - v1.Y) * t);
            int z = (int)(v1.Z + (v2.Z - v1.Z) * t);

            return new Vector3Int(x, y, z);
        }

        public override string ToString()
        {
            return $"{X}; {Y}; {Z}";
        }
    }
}
