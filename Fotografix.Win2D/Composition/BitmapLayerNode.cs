using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.ComponentModel;
using System.Drawing;

namespace Fotografix.Win2D.Composition
{
    internal sealed class BitmapLayerNode : LayerNode
    {
        private readonly BitmapLayer layer;
        private readonly OpacityEffect opacityEffect;
        private readonly CompositeEffectNode compositeEffectNode;
        private Win2DBitmap bitmap;

        public BitmapLayerNode(BitmapLayer layer, ICompositionRoot root) : base(layer, root)
        {
            this.layer = layer;
            layer.PropertyChanged += OnLayerPropertyChanged;

            this.opacityEffect = new OpacityEffect();

            this.compositeEffectNode = new CompositeEffectNode();
            UpdateBitmap();
        }

        public override void Dispose()
        {
            compositeEffectNode.Dispose();
            opacityEffect.Dispose();
            layer.PropertyChanged -= OnLayerPropertyChanged;
            bitmap.Dispose();
            base.Dispose();
        }

        protected override Rectangle Bounds => new Rectangle(Point.Empty, layer.Bitmap.Size);

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
                return compositeEffectNode.ResolveOutput(bitmapWithOpacity, background);
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

        private void OnLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BitmapLayer.Bitmap))
            {
                UpdateBitmap();
            }
        }

        private void UpdateBitmap()
        {
            bitmap?.Dispose();
            this.bitmap = new Win2DBitmap(layer.Bitmap, CanvasDevice.GetSharedDevice());
            UpdateOutput();
        }
    }
}
