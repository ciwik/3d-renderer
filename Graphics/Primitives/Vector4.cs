using System;

namespace Graphics.Primitives
{
    public struct Vector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = 1;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vector4 Normalized => this / Magnitude;

        public static Vector4 operator +(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
        }

        public static Vector4 operator -(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
        }

        public static Vector4 operator *(Vector4 v, float a)
        {
            return new Vector4(v.X * a, v.Y * a, v.Z * a, v.W * a);
        }

        public static Vector4 operator /(Vector4 v, float a)
        {
            return new Vector4(v.X / a, v.Y / a, v.Z / a, v.W / a);
        }

        public static Vector4 Lerp(Vector4 v1, Vector4 v2, float t)
        {
            float x = v1.X + (v2.X - v1.X) * t;
            float y = v1.Y + (v2.Y - v1.Y) * t;
            float z = v1.Z + (v2.Z - v1.Z) * t;
            float w = v1.W + (v2.W - v1.W) * t;

            return new Vector4(x, y, z, w);
        }

        public static float Dot(Vector4 v1, Vector4 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
        }

        public override string ToString()
        {
            return $"{X}; {Y}; {Z}; {W}";
        }
    }
}
