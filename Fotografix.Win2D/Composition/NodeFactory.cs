using Fotografix.Adjustments;
using Fotografix.Win2D.Composition.Adjustments;
using System;

namespace Fotografix.Win2D.Composition
{
    internal static class NodeFactory
    {
        internal static ImageNode CreateNode(Image image, ICompositionRoot root)
        {
            return new ImageNode(image, root);
        }

        internal static LayerNode CreateNode(Layer layer, ICompositionRoot root)
        {
            Visitor visitor = new Visitor { CompositionRoot = root };
            layer.Accept(visitor);
            return visitor.LayerNode ?? throw new InvalidOperationException("Unsupported layer: " + layer);
        }

        internal static AdjustmentNode CreateNode(Adjustment adjustment)
        {
            Visitor visitor = new Visitor();
            adjustment.Accept(visitor);
            return visitor.AdjustmentNode ?? throw new InvalidOperationException("Unsupported adjustment: " + adjustment);
        }

        private sealed class Visitor : ImageElementVisitor
        {
            internal ICompositionRoot CompositionRoot { get; set; }
            internal LayerNode LayerNode { get; private set; }
            internal AdjustmentNode AdjustmentNode { get; private set; }

            public override bool VisitEnter(AdjustmentLayer layer)
            {
                this.LayerNode = new AdjustmentLayerNode(layer, CompositionRoot);
                return false;
            }

            public override bool VisitEnter(BitmapLayer layer)
            {
                this.LayerNode = new BitmapLayerNode(layer, CompositionRoot);
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