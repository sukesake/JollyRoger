using SharpDX;

namespace JollyRoger
{
    public static class PointExtensions
    {
        public static Point Subtract (this Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y -b.Y);
        }
    }
}
