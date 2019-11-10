using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Composition
{
    public sealed class BitmapLayer : Layer
    {
        private readonly OpacityEffect opacityEffect;
        private readonly CompositeEffect compositeEffect;

        private readonly CanvasBitmap bitmap;

        public BitmapLayer(CanvasBitmap bitmap)
        {
            this.opacityEffect = new OpacityEffect();
            opacityEffect.Source = bitmap;

            this.compositeEffect = new CompositeEffect();
            // reserve background and foreground slots for the effect, actual sources are set later
            compositeEffect.Sources.Add(null);
            compositeEffect.Sources.Add(null);

            this.bitmap = bitmap;
            UpdateOutput();
        }

        public BitmapLayer(ICanvasResourceCreator resourceCreator, int width, int height)
            : this(new CanvasRenderTarget(resourceCreator, width, height, 96))
        {
        }

        public static async Task<BitmapLayer> LoadAsync(ICanvasResourceCreator resourceCreator, StorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(resourceCreator, stream);
                return new BitmapLayer(bitmap) { Name = file.DisplayName };
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            compositeEffect.Dispose();
            opacityEffect.Dispose();
            bitmap.Dispose();
        }

        public int Width => (int)bitmap.SizeInPixels.Width;
        public int Height => (int)bitmap.SizeInPixels.Height;

        protected override ICanvasImage ResolveOutput(ICanvasImage background)
        {
            if (!Visible || Opacity == 0)
            {
                return background;
            }

            ICanvasImage bitmapWithOpacity = ApplyOpacityToBitmap();

            if (background == null)
            {
                return bitmapWithOpacity;
            }

            if (BlendMode == BlendMode.Normal)
            {
                compositeEffect.Sources[0] = background;
                compositeEffect.Sources[1] = bitmapWithOpacity;
                return compositeEffect;
            }

            return Blend(bitmapWithOpacity, background);
        }

        private ICanvasImage ApplyOpacityToBitmap()
        {
            if (Opacity == 1)
            {
                return bitmap;
            }

            opacityEffect.Opacity = Opacity;
            return opacityEffect;
        }
    }
}
