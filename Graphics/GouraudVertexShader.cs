using System;
using System.Drawing;
using Graphics.Primitives;

namespace Graphics
{
    public class GouraudVertexShader : IPixelShader, IVertexShader
    {
        private Vector3 _varyingIntensity;
        private Vector3 _lightDirection;

        public Matrix4x4 View { get; set; } = Matrix4x4.Identity;
        public Matrix4x4 Viewport { get; set; } = Matrix4x4.Identity;

        public GouraudVertexShader(Vector3 lightDirection)
        {
            _lightDirection = lightDirection;
        }

        public bool OnPixel(Vector3 bar, ref Color color)
        {
            float intensity = Vector3.Dot(_varyingIntensity, bar);
            color = Color.FromArgb(255, (byte) (color.R * intensity), (byte) (color.G * intensity), (byte) (color.B * intensity));
            return false;
        }

        public Vector3 OnVertex(Polygon face, int vert)
        {
            Vector3 normal = face.Normals[vert];
            Vector3 vertex = face.Vertices[vert];
            _varyingIntensity[vert] = Math.Max(0, Vector3.Dot(normal, _lightDirection));
            Vector4 v4 = new Vector4(vertex);
            return (Viewport * View * v4).ToVector3();
        }
    }
}