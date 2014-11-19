using System.Runtime.InteropServices;

namespace RenderEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4B
    {
        public readonly byte X;
        public readonly byte Y;
        public readonly byte Z;
        public readonly byte W;

        public Vector4B(byte x, byte y, byte z, byte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}