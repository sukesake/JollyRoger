using System;
using System.Runtime.InteropServices;
using RenderEngine;
using SharpDX;

namespace Game
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BoundingFrustumF
    {
        public readonly PlaneF Left;
        public readonly PlaneF Right;
        public readonly PlaneF Top;
        public readonly PlaneF Bottom;
        public readonly PlaneF Near;
        public readonly PlaneF Far;

        public BoundingFrustumF(MatrixF value)
        {
            Left = PlaneF.CreateFrustrumPlane(
                new Vector3(-value.M14 - value.M11, -value.M24 - value.M21, -value.M34 - value.M31),
                -value.M44 - value.M41);

            Right = PlaneF.CreateFrustrumPlane(
                new Vector3(-value.M14 + value.M11, -value.M24 + value.M21, -value.M34 + value.M31),
                -value.M44 + value.M41);

            Top = PlaneF.CreateFrustrumPlane(
                new Vector3(-value.M14 + value.M12, -value.M24 + value.M22, -value.M34 + value.M32),
                -value.M44 + value.M42);

            Bottom = PlaneF.CreateFrustrumPlane(
                new Vector3(-value.M14 - value.M12, -value.M24 - value.M22, -value.M34 - value.M32),
                -value.M44 - value.M42);

            Near = PlaneF.CreateFrustrumPlane(
                new Vector3(-value.M13, -value.M23, -value.M33),
                -value.M43);

            Far = PlaneF.CreateFrustrumPlane(
                new Vector3(-value.M14 + value.M13, -value.M24 + value.M23, -value.M34 + value.M33),
                -value.M44 + value.M43);
        }

        public bool FastIntersects(BoundingBoxF aabb)
        {
            PlaneF plane; Vector3 normal, p;

            plane = Bottom;

            normal = plane.Normal * -1;

            //normal.X = -normal.X;
            //normal.Y = -normal.Y;
            //normal.Z = -normal.Z;

            p = aabb.GetPositiveVertex(normal);

            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
            {
                return false;
            }

            plane = Far;

            normal = plane.Normal * -1;

            //normal.X = -normal.X;
            //normal.Y = -normal.Y;
            //normal.Z = -normal.Z;

            p = aabb.GetPositiveVertex(normal);

            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
            {
                return false;
            }

            plane = Left;

            normal = plane.Normal * -1;

            //normal.X = -normal.X;
            //normal.Y = -normal.Y;
            //normal.Z = -normal.Z;

            p = aabb.GetPositiveVertex(normal);

            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
            {
                return false;
            }

            plane = Near;

            normal = plane.Normal * -1;

            //normal.X = -normal.X;
            //normal.Y = -normal.Y;
            //normal.Z = -normal.Z;

            p = aabb.GetPositiveVertex(normal);

            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
            {
                return false;
            }

            plane = Right;

            normal = plane.Normal * -1;

            //normal.X = -normal.X;
            //normal.Y = -normal.Y;
            //normal.Z = -normal.Z;

            p = aabb.GetPositiveVertex(normal);

            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
            {
                return false;
            }

            plane = Top;

            normal = plane.Normal * -1;

            //normal.X = -normal.X;
            //normal.Y = -normal.Y;
            //normal.Z = -normal.Z;

            p = aabb.GetPositiveVertex(normal);

            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
            {
                return false;
            }

            return true;
        }
    }
}