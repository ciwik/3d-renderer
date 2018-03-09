using System;

namespace Graphics.Primitives
{
    public class BarycentricVector {

        public float Magnitude { get; set; }
        public float Direction { get; set; }

        public BarycentricVector(float magnitude, float direction)
        {
            Magnitude = magnitude;
            Direction = direction;

            if (Magnitude < 0)
            {
                Magnitude = -Magnitude;
                Direction = (180 + Direction) % 360;
            }

            if (Direction < 0)
                Direction += 360;
        }

        public static BarycentricVector operator +(BarycentricVector a, BarycentricVector b)
        {
            double aX = a.Magnitude * Math.Cos(a.Direction);
            double aY = a.Magnitude * Math.Sin(a.Direction);

            double bX = b.Magnitude * Math.Cos(b.Direction);
            double bY = b.Magnitude * Math.Sin(b.Direction);
        
            aX += bX;
            aY += bY;

            double magnitude = Math.Sqrt(Math.Pow(aX, 2) + Math.Pow(aY, 2)), direction;
            if (magnitude < 0.0001)
            {
                direction = 0;
                magnitude = 0;
            }
            else
                direction = Math.Atan2(aY, aX);

            return new BarycentricVector((float)magnitude, (float)direction);
        }

        public static BarycentricVector operator *(BarycentricVector vector, float k)
        {
            return new BarycentricVector(vector.Magnitude * k, vector.Direction);
        }

        public Vector3 ToPoint(float z)
        {
            double aX = Magnitude * Math.Cos(Direction);
            double aY = Magnitude * Math.Sin(Direction);

            return new Vector3((float)aX, (float)aY, z);
        }

        private const float Diff = 0.001f;
        private static float Rad2Deg = (float)(180 / Math.PI);
        private static float GetBearingAngle(Vector3 start, Vector3 end)
        {
            Vector3 half = (start + end)/ 2f;

            Vector3 diff = half - start;
            
            if (diff.X < Diff)
                diff.X = Diff;

            if (diff.Y < Diff)
                diff.Y = Diff;

            float angle;
            if (Math.Abs(diff.X) > Math.Abs(diff.Y))
            {
                angle = (float)Math.Tanh(diff.Y / diff.X) * Rad2Deg;
                if ((diff.X < 0 && diff.Y > 0) || (diff.X < 0 && diff.Y < 0))
                    angle = 180;
            }
            else
            {
                angle = (float)Math.Tanh(diff.X / diff.Y) * Rad2Deg;
                if ((diff.Y < 0 && diff.X > 0) || (diff.Y < 0 && diff.X < 0))
                    angle = 180;
                angle = 90 - angle;
            }

            return angle;
        }
    }
}
