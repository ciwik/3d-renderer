using System.Drawing;

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
    }
}
