using System.Runtime.InteropServices;
using SharpDX;

namespace Game
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BoundingBoxF
    {
        public BoundingBoxF(Vector3 minimum, Vector3 maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public readonly Vector3 Maximum;
        public readonly Vector3 Minimum;

        public Vector3 GetPositiveVertex(Vector3 normal)
        {
            return
                new Vector3(
                    normal.X >= 0 ? Maximum.X : Minimum.X,
                    normal.Y >= 0 ? Maximum.Y : Minimum.Y,
                    normal.Z >= 0 ? Maximum.Z : Minimum.Z);
        }

        private Vector3 GetNegativeVertex(Vector3 normal)
        {
            return
                new Vector3(
                    normal.X >= 0 ? Minimum.X : Maximum.X,
                    normal.Y >= 0 ? Minimum.Y : Maximum.Y,
                    normal.Z >= 0 ? Minimum.Z : Maximum.Z);
        }
    }
}