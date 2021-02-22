using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;

namespace Fotografix.Win2D.Composition
{
    internal sealed class BitmapLayerNode : LayerNode
    {
        private readonly BitmapLayer layer;
        private readonly OpacityEffect opacityEffect;
        private readonly Transform2DEffect transformEffect;
        private readonly CompositeEffectNode compositeEffectNode;
        private Win2DBitmap bitmap;

        public BitmapLayerNode(BitmapLayer layer, NodeFactory nodeFactory) : base(layer, nodeFactory)
        {
            this.layer = layer;
            layer.PropertyChanged += Layer_PropertyChanged;

            this.opacityEffect = new OpacityEffect();
            this.transformEffect = new Transform2DEffect();
            this.compositeEffectNode = new CompositeEffectNode();

            UpdateBitmap();
            UpdateTransform();
        }

        public override void Dispose()
        {
            compositeEffectNode.Dispose();
            transformEffect.Dispose();
            opacityEffect.Dispose();
            layer.PropertyChanged -= Layer_PropertyChanged;
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

            transformEffect.Source = ApplyOpacityToBitmap();

            if (background == null)
            {
                return transformEffect;
            }

            if (layer.BlendMode == BlendMode.Normal)
            {
                return compositeEffectNode.ResolveOutput(transformEffect, background);
            }

            return Blend(transformEffect, background);
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

        private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BitmapLayer.Bitmap):
                    UpdateBitmap();
                    break;

                case nameof(Layer.Position):
                    UpdateTransform();
                    break;
            }
        }

        private void UpdateBitmap()
        {
            bitmap?.Dispose();
            this.bitmap = NodeFactory.WrapBitmap(layer.Bitmap);
            UpdateOutput();
        }

        private void UpdateTransform()
        {
            transformEffect.TransformMatrix = Matrix3x2.CreateTranslation(layer.Position.X, layer.Position.Y);
        }
    }
}
