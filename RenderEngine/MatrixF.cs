using System;
using System.Runtime.InteropServices;
using SharpDX;

namespace RenderEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MatrixF
    {
        #region Static methods

        public static MatrixF CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            float num1 = 1f / (float)Math.Tan((double)fieldOfView * 0.5);
            float num2 = num1 / aspectRatio;

            return new MatrixF(
                num2,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                num1,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                farPlaneDistance / (nearPlaneDistance - farPlaneDistance),
                -1f,
                0.0f,
                0.0f,

                (float)
                ((double)nearPlaneDistance * (double)farPlaneDistance /
                 ((double)nearPlaneDistance - (double)farPlaneDistance)),
                0.0f);
        }

        public static MatrixF CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            //TODO(PRUETT): These names are awful. figure out the math behind it and give it them better names
            Vector3 vector1 = (cameraPosition - cameraTarget);
            vector1.Normalize();
            Vector3 vector2 = Vector3.Cross(cameraUpVector, vector1);
            vector2.Normalize();
            Vector3 vector3 = Vector3.Cross(vector1, vector2);

            return new MatrixF(vector2.X, vector3.X, vector1.X, 0.0f, vector2.Y, vector3.Y, vector1.Y, 0.0f,
                               vector2.Z, vector3.Z, vector1.Z, 0.0f, -Vector3.Dot(vector2, cameraPosition),
                               -Vector3.Dot(vector3, cameraPosition), -Vector3.Dot(vector1, cameraPosition), 1f);
        }

        public static MatrixF CreateTranslation(float xPosition, float yPosition, float zPosition)
        {
            return new MatrixF(
                1f,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                1f,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                1f,
                0.0f,
                xPosition,
                yPosition,
                zPosition,
                1f);
        }

        public static MatrixF CreateRotationX(float radians)
        {
            float num1 = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);

            return
                new MatrixF(1f, 0.0f, 0.0f, 0.0f, 0.0f, num1, num2, 0.0f, 0.0f, -num2, num1, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
        }

        public static MatrixF CreateRotationY(float radians)
        {
            float num1 = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);

            return
                new MatrixF(num1, 0.0f, -num2, 0.0f, 0.0f, 1f, 0.0f, 0.0f, num2, 0.0f, num1, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
        }

        #endregion

        public override string ToString()
        {
            return
                string.Format(
                    "float4x4({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15})",
                    M11.ToString().Replace(",", "."),
                    M12.ToString().Replace(",", "."),
                    M13.ToString().Replace(",", "."),
                    M14.ToString().Replace(",", "."),
                    M21.ToString().Replace(",", "."),
                    M22.ToString().Replace(",", "."),
                    M23.ToString().Replace(",", "."),
                    M24.ToString().Replace(",", "."),
                    M31.ToString().Replace(",", "."),
                    M32.ToString().Replace(",", "."),
                    M33.ToString().Replace(",", "."),
                    M34.ToString().Replace(",", "."),
                    M41.ToString().Replace(",", "."),
                    M42.ToString().Replace(",", "."),
                    M43.ToString().Replace(",", "."),
                    M44.ToString().Replace(",", ".")
                    );
        }

        public readonly float M11;
        public readonly float M12;
        public readonly float M13;
        public readonly float M14;
        public readonly float M21;
        public readonly float M22;
        public readonly float M23;
        public readonly float M24;
        public readonly float M31;
        public readonly float M32;
        public readonly float M33;
        public readonly float M34;
        public readonly float M41;
        public readonly float M42;
        public readonly float M43;
        public readonly float M44;

        public MatrixF(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        public static readonly MatrixF Identity = new MatrixF(1f, 0, 0, 0, 0, 1f, 0, 0, 0, 0, 1f, 0, 0, 0, 0, 1f);

        public MatrixF(MatrixF left, MatrixF right)
        {
            float num1 = right.M21;
            float num2 = left.M12;
            float num3 = left.M11;
            float num4 = right.M11;
            float num5 = right.M31;
            float num6 = left.M13;
            float num7 = right.M41;
            float num8 = left.M14;
            M11 = (float)((double)num4 * (double)num3 + (double)num2 * (double)num1 + (double)num6 * (double)num5 + (double)num8 * (double)num7);
            float num9 = right.M22;
            float num10 = right.M12;
            float num11 = right.M32;
            float num12 = right.M42;
            M12 = (float)((double)num10 * (double)num3 + (double)num9 * (double)num2 + (double)num11 * (double)num6 + (double)num12 * (double)num8);
            float num13 = right.M23;
            float num14 = right.M13;
            float num15 = right.M33;
            float num16 = right.M43;
            M13 = (float)((double)num14 * (double)num3 + (double)num13 * (double)num2 + (double)num15 * (double)num6 + (double)num16 * (double)num8);
            float num17 = right.M24;
            float num18 = right.M14;
            float num19 = right.M34;
            float num20 = right.M44;
            M14 = (float)((double)num18 * (double)num3 + (double)num17 * (double)num2 + (double)num19 * (double)num6 + (double)num20 * (double)num8);
            float num21 = left.M22;
            float num22 = left.M21;
            float num23 = left.M23;
            float num24 = left.M24;
            M21 = (float)((double)num22 * (double)num4 + (double)num21 * (double)num1 + (double)num23 * (double)num5 + (double)num24 * (double)num7);
            M22 = (float)((double)num22 * (double)num10 + (double)num21 * (double)num9 + (double)num23 * (double)num11 + (double)num24 * (double)num12);
            M23 = (float)((double)num22 * (double)num14 + (double)num21 * (double)num13 + (double)num23 * (double)num15 + (double)num24 * (double)num16);
            M24 = (float)((double)num22 * (double)num18 + (double)num21 * (double)num17 + (double)num23 * (double)num19 + (double)num24 * (double)num20);
            float num25 = left.M32;
            float num26 = left.M31;
            float num27 = left.M33;
            float num28 = left.M34;
            M31 = (float)((double)num26 * (double)num4 + (double)num25 * (double)num1 + (double)num27 * (double)num5 + (double)num28 * (double)num7);
            M32 = (float)((double)num26 * (double)num10 + (double)num25 * (double)num9 + (double)num27 * (double)num11 + (double)num28 * (double)num12);
            M33 = (float)((double)num26 * (double)num14 + (double)num25 * (double)num13 + (double)num27 * (double)num15 + (double)num28 * (double)num16);
            M34 = (float)((double)num26 * (double)num18 + (double)num25 * (double)num17 + (double)num27 * (double)num19 + (double)num28 * (double)num20);
            float num29 = left.M42;
            float num30 = left.M41;
            float num31 = left.M43;
            float num32 = left.M44;
            M41 = (float)((double)num30 * (double)num4 + (double)num29 * (double)num1 + (double)num31 * (double)num5 + (double)num32 * (double)num7);
            M42 = (float)((double)num30 * (double)num10 + (double)num29 * (double)num9 + (double)num31 * (double)num11 + (double)num32 * (double)num12);
            M43 = (float)((double)num30 * (double)num14 + (double)num29 * (double)num13 + (double)num31 * (double)num15 + (double)num32 * (double)num16);
            M44 = (float)((double)num30 * (double)num18 + (double)num29 * (double)num17 + (double)num31 * (double)num19 + (double)num32 * (double)num20);
        }

        public static MatrixF operator *(MatrixF left, MatrixF right)
        {
            return new MatrixF(
                (float)((double)right.M21 * (double)left.M12 + (double)left.M11 * (double)right.M11 + (double)right.M31 * (double)left.M13 + (double)right.M41 * (double)left.M14),
                (float)((double)right.M22 * (double)left.M12 + (double)right.M12 * (double)left.M11 + (double)right.M32 * (double)left.M13 + (double)right.M42 * (double)left.M14),
                (float)((double)right.M23 * (double)left.M12 + (double)right.M13 * (double)left.M11 + (double)right.M33 * (double)left.M13 + (double)right.M43 * (double)left.M14),
                (float)((double)right.M24 * (double)left.M12 + (double)right.M14 * (double)left.M11 + (double)right.M34 * (double)left.M13 + (double)right.M44 * (double)left.M14),
                (float)((double)left.M22 * (double)right.M21 + (double)left.M21 * (double)right.M11 + (double)left.M23 * (double)right.M31 + (double)left.M24 * (double)right.M41),
                (float)((double)left.M22 * (double)right.M22 + (double)left.M21 * (double)right.M12 + (double)left.M23 * (double)right.M32 + (double)left.M24 * (double)right.M42),
                (float)((double)right.M23 * (double)left.M22 + (double)right.M13 * (double)left.M21 + (double)right.M33 * (double)left.M23 + (double)left.M24 * (double)right.M43),
                (float)((double)right.M24 * (double)left.M22 + (double)right.M14 * (double)left.M21 + (double)right.M34 * (double)left.M23 + (double)right.M44 * (double)left.M24),
                (float)((double)left.M32 * (double)right.M21 + (double)left.M31 * (double)right.M11 + (double)left.M33 * (double)right.M31 + (double)left.M34 * (double)right.M41),
                (float)((double)left.M32 * (double)right.M22 + (double)left.M31 * (double)right.M12 + (double)left.M33 * (double)right.M32 + (double)left.M34 * (double)right.M42),
                (float)((double)right.M23 * (double)left.M32 + (double)left.M31 * (double)right.M13 + (double)left.M33 * (double)right.M33 + (double)left.M34 * (double)right.M43),
                (float)((double)right.M24 * (double)left.M32 + (double)right.M14 * (double)left.M31 + (double)right.M34 * (double)left.M33 + (double)right.M44 * (double)left.M34),
                (float)((double)left.M42 * (double)right.M21 + (double)left.M41 * (double)right.M11 + (double)left.M43 * (double)right.M31 + (double)left.M44 * (double)right.M41),
                (float)((double)left.M42 * (double)right.M22 + (double)left.M41 * (double)right.M12 + (double)left.M43 * (double)right.M32 + (double)left.M44 * (double)right.M42),
                (float)((double)right.M23 * (double)left.M42 + (double)left.M41 * (double)right.M13 + (double)left.M43 * (double)right.M33 + (double)left.M44 * (double)right.M43),
                (float)((double)right.M24 * (double)left.M42 + (double)left.M41 * (double)right.M14 + (double)right.M34 * (double)left.M43 + (double)left.M44 * (double)right.M44)
            );
        }
    }
}
