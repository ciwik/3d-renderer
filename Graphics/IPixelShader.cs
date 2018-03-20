using System.Drawing;
using Graphics.Primitives;

namespace Graphics
{
    public interface IPixelShader
    {
        bool OnPixel(Vector3 bar, ref Color color);
    }
}
