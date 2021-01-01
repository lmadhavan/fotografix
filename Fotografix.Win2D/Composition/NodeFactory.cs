using System;
using System.Collections.Generic;
using Fotografix.Adjustments;
using Fotografix.Win2D.Composition.Adjustments;

namespace Fotografix.Win2D.Composition
{
    internal class NodeFactory<BaseSourceType, BaseNodeType>
    {
        private readonly Dictionary<Type, Type> nodeTypeDictionary = new Dictionary<Type, Type>();

        public BaseNodeType Create(BaseSourceType source)
        {
            return (BaseNodeType)Activator.CreateInstance(nodeTypeDictionary[source.GetType()], source);
        }

        public BaseNodeType Create(BaseSourceType source, ICompositionRoot root)
        {
            return (BaseNodeType)Activator.CreateInstance(nodeTypeDictionary[source.GetType()], source, root);
        }

        internal void Register<SourceType, NodeType>() where SourceType : BaseSourceType where NodeType : BaseNodeType
        {
            nodeTypeDictionary[typeof(SourceType)] = typeof(NodeType);
        }
    }

    internal static class NodeFactory
    {
        internal static readonly NodeFactory<Layer, LayerNode> Layer = new NodeFactory<Layer, LayerNode>();
        internal static readonly NodeFactory<Adjustment, AdjustmentNode> Adjustment = new NodeFactory<Adjustment, AdjustmentNode>();

        static NodeFactory()
        {
            Layer.Register<AdjustmentLayer, AdjustmentLayerNode>();
            Layer.Register<BitmapLayer, BitmapLayerNode>();

            Adjustment.Register<BlackAndWhiteAdjustment, BlackAndWhiteAdjustmentNode>();
            Adjustment.Register<BrightnessContrastAdjustment, BrightnessContrastAdjustmentNode>();
            Adjustment.Register<GradientMapAdjustment, GradientMapAdjustmentNode>();
            Adjustment.Register<HueSaturationAdjustment, HueSaturationAdjustmentNode>();
            Adjustment.Register<LevelsAdjustment, LevelsAdjustmentNode>();
        }
    }
}