using System.Runtime.InteropServices;

namespace RenderEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3B
    {
        public readonly byte X;
        public readonly byte Y;
        public readonly byte Z;

        public Vector3B(byte x, byte y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
