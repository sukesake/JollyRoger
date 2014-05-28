using SharpDX;

namespace RenderEngine
{
    public static class ColorExtensions
    {
        public static Color4 ToColor4(this Color color)
        {
            return new Color4(color.R, color.G, color.B, color.A);
        }
    }
}
