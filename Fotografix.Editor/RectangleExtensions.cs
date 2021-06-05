using System;
using System.Drawing;

namespace Fotografix.Editor
{
    public static class RectangleExtensions
    {
        public static Rectangle Normalize(this Rectangle rect)
        {
            int l = Math.Min(rect.Left, rect.Right);
            int t = Math.Min(rect.Top, rect.Bottom);
            int r = Math.Max(rect.Left, rect.Right);
            int b = Math.Max(rect.Top, rect.Bottom);
            return Rectangle.FromLTRB(l, t, r, b);
        }
    }
}
