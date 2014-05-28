using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D10;

namespace RenderEngine
{
    public class BlockVertex
    {
        public static InputElement[] InputElements
        {
            get
            {
                return new[]
                           {
                               new InputElement("POSITION", 0,
                                                Format.
                                                    R32G32B32A32_Float,
                                                0, 0),
                               new InputElement("NORMAL", 0,
                                                Format.
                                                    R32G32B32_Float,
                                                Vector4.SizeInBytes, 0),
                               new InputElement("TEXCOORD", 0,
                                                Format.
                                                    R32G32_Float,
                                                Vector4.SizeInBytes + Vector3.SizeInBytes, 0)
                           };
            }
        }

        public static readonly int BlockVertexSize = Vector4.SizeInBytes + Vector3.SizeInBytes + Vector2.SizeInBytes;


    }
}