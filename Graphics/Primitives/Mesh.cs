using System;
using System.Drawing;
using System.Linq;

namespace Graphics.Primitives
{
    public struct Mesh
    {
        private Polygon[] _polygons;

        public Polygon[] Polygons => _polygons;
        public Bitmap Texture { get; set; }

        public Mesh(Polygon[] polygons)
        {
            _polygons = polygons;
            Texture = null;
        }

        public Mesh Normalize()
        {
            float max = _polygons.Max(p => p.Vertices.Max(v => Math.Max(v.X, Math.Max(v.Y, v.Z))));
            foreach (var polygon in _polygons)
            {
                for (int i = 0; i < polygon.Vertices.Length; i++)
                {
                    polygon.Vertices[i] /= max;
                }
            }

            return this;
        }
    }
}
