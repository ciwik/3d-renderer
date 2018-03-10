namespace Graphics.Primitives
{
    public struct Polygon
    {
        private Vector3[] _vertices;
        private Vector3[] _normals;
        private Vector2[] _uvs;

        public Vector3[] Vertices => _vertices;
        public Vector3[] Normals => _normals;
        public Vector2[] UVs => _uvs;

        public Polygon(Vector3[] vertices, Vector3[] normals, Vector2[] uvs)
        {
            _vertices = vertices;
            _normals = normals;
            _uvs = uvs;
        }
    }
}
