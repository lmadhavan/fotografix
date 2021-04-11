using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Composition
{
    internal sealed class BitmapBlendingStrategy : IBlendingStrategy
    {
        private readonly Win2DBitmap bitmapNode;
        private readonly IComposableNode drawingPreviewNode;

        private readonly BlendEffectNode blendEffectNode;
        private readonly OpacityEffect opacityEffect;
        private readonly CompositeEffectNode compositeEffectNode;

        internal BitmapBlendingStrategy(Win2DBitmap bitmapNode, IComposableNode drawingPreviewNode)
        {
            this.bitmapNode = bitmapNode;
            this.drawingPreviewNode = drawingPreviewNode;

            this.blendEffectNode = new BlendEffectNode();
            this.opacityEffect = new OpacityEffect();
            this.compositeEffectNode = new CompositeEffectNode();
        }

        public void Dispose()
        {
            compositeEffectNode.Dispose();
            opacityEffect.Dispose();
            blendEffectNode.Dispose();
            bitmapNode.Dispose();
        }

        public ICanvasImage Blend(Layer layer, ICanvasImage background)
        {
            if (!layer.Visible || layer.Opacity == 0)
            {
                return background;
            }

            ICanvasImage content = drawingPreviewNode.Compose(bitmapNode.Output);
            ICanvasImage foreground = ApplyOpacity(content, layer.Opacity);
            return BlendBitmap(foreground, background, layer.BlendMode);
        }

        private ICanvasImage BlendBitmap(ICanvasImage foreground, ICanvasImage background, BlendMode blendMode)
        {
            if (background == null)
            {
                return foreground;
            }

            if (blendMode == BlendMode.Normal)
            {
                return compositeEffectNode.ResolveOutput(foreground, background);
            }

            return blendEffectNode.Blend(foreground, background, blendMode);
        }

        private ICanvasImage ApplyOpacity(ICanvasImage content, float opacity)
        {
            if (opacity == 1)
            {
                return content;
            }

            opacityEffect.Source = content;
            opacityEffect.Opacity = opacity;
            return opacityEffect;
        }
    }
}
