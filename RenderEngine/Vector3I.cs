using System;
using System.Runtime.InteropServices;

namespace RenderEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3I : IEquatable<Vector3I>
    {
        public readonly static Vector3I Zero = new Vector3I(0, 0, 0);

        public readonly uint X;
        public readonly uint Y;
        public readonly uint Z;

        public Vector3I(uint x, uint y, uint z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3I operator +(VectorIndex a, Vector3I b)
        {
            return new Vector3I(a.X + b.X, b.Y, a.Z + b.Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Vector3I)) return false;
            return Equals((Vector3I) obj);
        }

        public bool Equals(Vector3I other)
        {
            return other.X == X && other.Y == Y && other.Z == Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result*397) ^ Y.GetHashCode();
                result = (result*397) ^ Z.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(Vector3I left, Vector3I right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3I left, Vector3I right)
        {
            return !left.Equals(right);
        }
    }
}
