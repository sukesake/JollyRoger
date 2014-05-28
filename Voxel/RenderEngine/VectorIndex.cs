using System.Runtime.InteropServices;

namespace RenderEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public class VectorIndex
    {
        public readonly uint X;
        public readonly uint Z;

        public VectorIndex(uint x, uint z)
        {
            X = x;
            Z = z;
        }
    }
}
