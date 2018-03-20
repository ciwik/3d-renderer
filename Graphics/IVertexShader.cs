using Graphics.Primitives;

namespace Graphics
{
    public interface IVertexShader
    {
        Vector3 OnVertex(Polygon face, int vert);
    }
}
