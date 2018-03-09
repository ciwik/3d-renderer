using System;

namespace Graphics.Primitives
{
    public struct Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Vector2 v)
        {
            X = v.X;
            Y = v.Y;
            Z = 0;
        }

        public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vector3 Normalized => this / Magnitude;

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator *(Vector3 v, float a)
        {
            return new Vector3(v.X * a, v.Y * a, v.Z * a);
        }

        public static Vector3 operator /(Vector3 v, float a)
        {
            return new Vector3(v.X / a, v.Y / a, v.Z / a);
        }

        public static Vector3 Lerp(Vector3 v1, Vector3 v2, float t)
        {
            float x = v1.X + (v2.X - v1.X) * t;
            float y = v1.Y + (v2.Y - v1.Y) * t;
            float z = v1.Z + (v2.Z - v1.Z) * t;

            return new Vector3(x, y, z);
        }

        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X);
        }        

        public override string ToString()
        {
            return $"{X}; {Y}; {Z}";
        }
    }
}
