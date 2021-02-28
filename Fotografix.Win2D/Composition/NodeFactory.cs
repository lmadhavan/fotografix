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
            Visitor visitor = new Visitor(this);
            layer.Accept(visitor);
            return visitor.LayerNode ?? throw new InvalidOperationException("Unsupported layer: " + layer);
        }

        internal AdjustmentNode CreateAdjustmentNode(Adjustment adjustment)
        {
            Visitor visitor = new Visitor(this);
            adjustment.Accept(visitor);
            return visitor.AdjustmentNode ?? throw new InvalidOperationException("Unsupported adjustment: " + adjustment);
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

        private sealed class Visitor : ImageElementVisitor
        {
            private readonly NodeFactory nodeFactory;

            internal Visitor(NodeFactory nodeFactory)
            {
                this.nodeFactory = nodeFactory;
            }

            internal LayerNode LayerNode { get; private set; }
            internal AdjustmentNode AdjustmentNode { get; private set; }

            public override bool VisitEnter(AdjustmentLayer layer)
            {
                this.LayerNode = new AdjustmentLayerNode(layer, nodeFactory);
                return false;
            }

            public override bool VisitEnter(BitmapLayer layer)
            {
                this.LayerNode = new BitmapLayerNode(layer, nodeFactory);
                return false;
            }

            public override bool Visit(BlackAndWhiteAdjustment adjustment)
            {
                this.AdjustmentNode = new BlackAndWhiteAdjustmentNode(adjustment);
                return false;
            }

            public override bool Visit(BrightnessContrastAdjustment adjustment)
            {
                this.AdjustmentNode = new BrightnessContrastAdjustmentNode(adjustment);
                return false;
            }

            public override bool Visit(GradientMapAdjustment adjustment)
            {
                this.AdjustmentNode = new GradientMapAdjustmentNode(adjustment);
                return false;
            }

            public override bool Visit(HueSaturationAdjustment adjustment)
            {
                this.AdjustmentNode = new HueSaturationAdjustmentNode(adjustment);
                return false;
            }

            public override bool Visit(LevelsAdjustment adjustment)
            {
                this.AdjustmentNode = new LevelsAdjustmentNode(adjustment);
                return false;
            }
        }
    }
}