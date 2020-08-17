namespace Fotografix.Win2D
{
    internal static class ConversionExtensions
    {
        public static Windows.UI.Color ToWindowsColor(this System.Drawing.Color color)
        {
            return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Numerics.Vector2 ToVector2(this System.Drawing.Point pt)
        {
            return new System.Numerics.Vector2(pt.X, pt.Y);
        }

        public static Windows.Foundation.Rect ToWindowsRect(this System.Drawing.Rectangle rect)
        {
            return new Windows.Foundation.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
