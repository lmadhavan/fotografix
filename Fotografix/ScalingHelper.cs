using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace Fotografix
{
    public sealed class ScalingHelper
    {
        private readonly ICanvasResourceCreatorWithDpi dpiProvider;
        private readonly IRenderScaleProvider renderScaleProvider;

        public ScalingHelper(ICanvasResourceCreatorWithDpi dpiProvider, IRenderScaleProvider renderScaleProvider)
        {
            this.dpiProvider = dpiProvider;
            this.renderScaleProvider = renderScaleProvider;
        }

        private double DpiScalingFactor => dpiProvider.Dpi / 96;
        private double RenderScalingFactor => DpiScalingFactor / renderScaleProvider.RenderScale;

        public Size ViewportToScreen(Size size)
        {
            return new Size(
                size.Width / DpiScalingFactor,
                size.Height / DpiScalingFactor
            );
        }

        public Size ScreenToViewport(Size size)
        {
            return new Size(
                size.Width * DpiScalingFactor,
                size.Height * DpiScalingFactor
            );
        }

        public double ScreenToImage(double value)
        {
            return value * RenderScalingFactor;
        }

        public Point ScreenToImage(Point pt)
        {
            return new Point(
                pt.X * RenderScalingFactor,
                pt.Y * RenderScalingFactor
            );
        }

        public Rect ImageToScreen(Rect rect)
        {
            return new Rect(
                rect.X / RenderScalingFactor,
                rect.Y / RenderScalingFactor,
                rect.Width / RenderScalingFactor,
                rect.Height / RenderScalingFactor
            );
        }
    }
}
