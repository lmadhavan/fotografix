using Fotografix.Adjustments;
using Fotografix.Drawing;
using Fotografix.Editor;
using Fotografix.Win2D.Composition.Adjustments;
using Microsoft.Graphics.Canvas;
using System;
using System.Drawing;

namespace Fotografix.Win2D.Composition
{
    internal sealed class NodeFactory
    {
        private readonly ICanvasResourceCreator resourceCreator;
        private readonly int transparencyGridSize;
        private readonly bool interactiveMode;

        internal NodeFactory(ICanvasResourceCreator resourceCreator, int transparencyGridSize, bool interactiveMode)
        {
            this.resourceCreator = resourceCreator;
            this.transparencyGridSize = transparencyGridSize;
            this.interactiveMode = interactiveMode;
        }

        internal event EventHandler Invalidated;

        internal void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        internal ImageNode CreateImageNode(Image image)
        {
            return new ImageNode(image, this);
        }

        internal LayerNode CreateLayerNode(Layer layer)
        {
            return layer switch
            {
                BitmapLayer l => new BitmapLayerNode(l, this),
                AdjustmentLayer l => new AdjustmentLayerNode(l, this),
                _ => throw new InvalidOperationException("Unsupported layer: " + layer)
            };
        }

        internal AdjustmentNode CreateAdjustmentNode(Adjustment adjustment)
        {
            return adjustment switch
            {
                BlackAndWhiteAdjustment a => new BlackAndWhiteAdjustmentNode(a),
                BrightnessContrastAdjustment a => new BrightnessContrastAdjustmentNode(a),
                GradientMapAdjustment a => new GradientMapAdjustmentNode(a),
                HueSaturationAdjustment a => new HueSaturationAdjustmentNode(a),
                LevelsAdjustment a => new LevelsAdjustmentNode(a),
                _ => throw new InvalidOperationException("Unsupported adjustment: " + adjustment)
            };
        }

        internal IDrawableNode CreateTransparencyGridNode()
        {
            if (transparencyGridSize > 0)
            {
                return new TransparencyGridNode(transparencyGridSize, resourceCreator);
            }

            return new NullDrawableNode();
        }

        internal IDrawableNode CreateCropPreviewNode(Image image, Viewport viewport)
        {
            if (interactiveMode)
            {
                return new CropPreviewNode(image, resourceCreator, viewport);
            }

            return new NullDrawableNode();
        }

        internal IComposableNode CreateDrawingPreviewNode(IDrawable drawable)
        {
            if (interactiveMode)
            {
                return new DrawingPreviewNode(drawable, resourceCreator);
            }

            return new NullComposableNode();
        }

        internal Win2DBitmap CreateBitmap(Size size)
        {
            return new Win2DBitmap(size, resourceCreator);
        }

        internal Win2DBitmap WrapBitmap(Bitmap bitmap)
        {
            return new Win2DBitmap(bitmap, resourceCreator);
        }
    }
}