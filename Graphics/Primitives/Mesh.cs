namespace Graphics.Primitives
{
    public struct Mesh
    {
        private Polygon[] _polygons;

        public Polygon[] Polygons => _polygons;

        public Mesh(Polygon[] polygons)
        {
            _polygons = polygons;
        }
    }
}
