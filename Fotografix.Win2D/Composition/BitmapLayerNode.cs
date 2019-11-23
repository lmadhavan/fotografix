using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.ComponentModel;

namespace Fotografix.Win2D.Composition
{
    internal sealed class BitmapLayerNode : LayerNode
    {
        private readonly BitmapLayer layer;
        private readonly OpacityEffect opacityEffect;
        private readonly CompositeEffect compositeEffect;
        private BitmapNode bitmap;

        public BitmapLayerNode(BitmapLayer layer) : base(layer)
        {
            this.layer = layer;

            this.opacityEffect = new OpacityEffect();

            this.compositeEffect = new CompositeEffect();
            // reserve background and foreground slots for the effect, actual sources are set later
            compositeEffect.Sources.Add(null);
            compositeEffect.Sources.Add(null);

            this.bitmap = (BitmapNode)layer.Bitmap;
            UpdateOutput();
        }

        public override void Dispose()
        {
            compositeEffect.Dispose();
            opacityEffect.Dispose();
            base.Dispose();
        }

        protected override void OnLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BitmapLayer.Bitmap))
            {
                this.bitmap = (BitmapNode)layer.Bitmap;
            }

            base.OnLayerPropertyChanged(sender, e);
        }

        protected override ICanvasImage ResolveOutput(ICanvasImage background)
        {
            if (!layer.Visible || layer.Opacity == 0)
            {
                return background;
            }

            ICanvasImage bitmapWithOpacity = ApplyOpacityToBitmap();

            if (background == null)
            {
                return bitmapWithOpacity;
            }

            if (layer.BlendMode == BlendMode.Normal)
            {
                compositeEffect.Sources[0] = background;
                compositeEffect.Sources[1] = bitmapWithOpacity;
                return compositeEffect;
            }

            return Blend(bitmapWithOpacity, background);
        }

        private ICanvasImage ApplyOpacityToBitmap()
        {
            if (layer.Opacity == 1)
            {
                return bitmap.Output;
            }

            opacityEffect.Source = bitmap.Output;
            opacityEffect.Opacity = layer.Opacity;
            return opacityEffect;
        }
    }
}
