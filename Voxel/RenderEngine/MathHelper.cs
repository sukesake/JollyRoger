namespace RenderEngine
{
    public class MathHelper
    {
        public const float E = 2.718282f;
        public const float Log2E = 1.442695f;
        public const float Log10E = 0.4342945f;
        public const float Pi = 3.141593f;
        public const float TwoPi = 6.283185f;
        public const float PiOver2 = 1.570796f;
        public const float PiOver4 = 0.7853982f;

        public static float ToRadians(float degrees)
        {
            return degrees * 0.01745329f;
        }

        public static float ToDegrees(float radians)
        {
            return radians * 57.29578f;
        }

        public static int BytesToInt(byte[] bytes, int offset)
        {
            int ret = 0;

            for (int i = 0; i < 4 && i + offset < bytes.Length; i++)
            {
                ret <<= 8;
                ret |= bytes[i] & 0xFF;
            }

            return ret;
        }
    }
}
