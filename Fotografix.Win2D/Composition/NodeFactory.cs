﻿using System;
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
            Type sourceType = source.GetType();

            if (nodeTypeDictionary.TryGetValue(sourceType, out Type nodeType))
            {
                return (BaseNodeType)Activator.CreateInstance(nodeType, source);
            }

            throw new ArgumentException("Unsupported type " + sourceType);
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
            Adjustment.Register<GradientMapAdjustment, GradientMapAdjustmentNode>();
            Adjustment.Register<HueSaturationAdjustment, HueSaturationAdjustmentNode>();
            Adjustment.Register<BrightnessContrastAdjustment, BrightnessContrastAdjustmentNode>();
        }
    }
}