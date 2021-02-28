using Fotografix.Drawing;
using Fotografix.Editor;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
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
        private IComposableNode drawingPreviewNode;

        public BitmapLayerNode(BitmapLayer layer, NodeFactory nodeFactory) : base(layer, nodeFactory)
        {
            this.layer = layer;
            layer.PropertyChanged += Layer_PropertyChanged;
            layer.UserPropertyChanged += Layer_PropertyChanged;

            this.opacityEffect = new OpacityEffect();
            this.transformEffect = new Transform2DEffect();
            this.compositeEffectNode = new CompositeEffectNode();

            UpdateBitmap();
            UpdateTransform();
            UpdatePreview();
            UpdateOutput();
        }

        public override void Dispose()
        {
            drawingPreviewNode.Dispose();
            compositeEffectNode.Dispose();
            transformEffect.Dispose();
            opacityEffect.Dispose();

            layer.UserPropertyChanged -= Layer_PropertyChanged;
            layer.PropertyChanged -= Layer_PropertyChanged;

            bitmap.Dispose();
            base.Dispose();
        }

        protected override ICanvasImage ResolveOutput(ICanvasImage background)
        {
            if (!layer.Visible || layer.Opacity == 0)
            {
                return background;
            }

            transformEffect.Source = bitmap.Output;
            ICanvasImage content = drawingPreviewNode.Compose(transformEffect);
            ICanvasImage foreground = ApplyOpacityTo(content);
            return BlendBitmap(foreground, background);
        }

        private ICanvasImage BlendBitmap(ICanvasImage foreground, ICanvasImage background)
        {
            if (background == null)
            {
                return foreground;
            }

            if (layer.BlendMode == BlendMode.Normal)
            {
                return compositeEffectNode.ResolveOutput(foreground, background);
            }

            return Blend(foreground, background);
        }

        private ICanvasImage ApplyOpacityTo(ICanvasImage content)
        {
            if (layer.Opacity == 1)
            {
                return content;
            }

            opacityEffect.Source = content;
            opacityEffect.Opacity = layer.Opacity;
            return opacityEffect;
        }

        private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == EditorProperties.DrawingPreview)
            {
                UpdatePreview();
            }
            else
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

            UpdateOutput();
        }

        private void UpdateBitmap()
        {
            bitmap?.Dispose();
            this.bitmap = NodeFactory.WrapBitmap(layer.Bitmap);
        }

        private void UpdateTransform()
        {
            transformEffect.TransformMatrix = Matrix3x2.CreateTranslation(layer.Position.X, layer.Position.Y);
        }

        private void UpdatePreview()
        {
            IDrawable drawable = layer.GetDrawingPreview();
            drawingPreviewNode?.Dispose();

            if (drawable != null)
            {
                this.drawingPreviewNode = NodeFactory.CreateDrawingPreviewNode(drawable);
                drawingPreviewNode.Invalidated += Preview_Invalidated;
            }
            else
            {
                this.drawingPreviewNode = new NullComposableNode();
            }
        }

        private void Preview_Invalidated(object sender, EventArgs e)
        {
            UpdateOutput();
            NodeFactory.Invalidate();
        }
    }
}
