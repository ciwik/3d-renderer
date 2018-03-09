namespace Graphics.Primitives
{
    public struct Polygon
    {
        private Vector3[] _vertices;
        private Vector3 _normal;

        public Vector3[] Vertices => _vertices;

        public Vector3 Normal => _normal;

        public Polygon(Vector3[] vertices, Vector3 normal)
        {
            _vertices = vertices;
            _normal = normal;
        }
    }
}
