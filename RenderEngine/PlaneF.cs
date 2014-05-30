using System.Runtime.InteropServices;
using SharpDX;

namespace RenderEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PlaneF
    {
        public static PlaneF CreateFrustrumPlane(Vector3 normal, float d)
        {
            return new PlaneF(normal / normal.Length(), d / normal.Length());
        }

        private PlaneF(Vector3 normal, float d)
        {
            Normal = normal;
            D = d;
        }

        public readonly Vector3 Normal;

        public readonly float D;
    }
}