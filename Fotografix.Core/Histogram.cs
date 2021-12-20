using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Foundation;
using Windows.UI;

namespace Fotografix
{
    public sealed class Histogram
    {
        private const int NumBins = 256;
        private const int Height = 100;

        private readonly float[] r;
        private readonly float[] g;
        private readonly float[] b;

        public Histogram(float[] r, float[] g, float[] b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public static Histogram Compute(ICanvasImage image, Rect rect, ICanvasResourceCreator resourceCreator)
        {
            float[] r = CanvasImage.ComputeHistogram(image, rect, resourceCreator, EffectChannelSelect.Red, NumBins);
            float[] g = CanvasImage.ComputeHistogram(image, rect, resourceCreator, EffectChannelSelect.Green, NumBins);
            float[] b = CanvasImage.ComputeHistogram(image, rect, resourceCreator, EffectChannelSelect.Blue, NumBins);
            return new Histogram(r, g, b);
        }

        public static Size RenderSize => new Size(NumBins, Height);

        public void Draw(CanvasDrawingSession ds)
        {
            float s = Height / 0.025f;

            void Draw(float[] x, Color color)
            {
                for (int i = 0; i < NumBins; i++)
                {
                    ds.DrawLine(i, Height, i, Height - s * x[i], color);
                }
            }

            ds.Blend = CanvasBlend.Add;
            Draw(r, Color.FromArgb(255, 255, 0, 0));
            Draw(g, Color.FromArgb(255, 0, 255, 0));
            Draw(b, Color.FromArgb(255, 0, 0, 255));
        }

        public CanvasBitmap ExportToCanvasBitmap(ICanvasResourceCreator resourceCreator)
        {
            var rt = new CanvasRenderTarget(resourceCreator, NumBins, Height, 96);

            using (var ds = rt.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                Draw(ds);
            }

            return rt;
        }
    }
}
