using System;
using System.Collections.Generic;
using System.Linq;

namespace Fotografix.Win2D.Composition
{
    internal class NodeFactory<BaseSourceType, BaseNodeType>
    {
        private readonly Dictionary<Type, Type> nodeTypeDictionary = new Dictionary<Type, Type>();

        public BaseNodeType Create(BaseSourceType source)
        {
            Type sourceType = source.GetType();
            Type[] interfaces = sourceType.GetInterfaces();

            foreach (Type type in interfaces.Concat(new Type[] { sourceType }))
            {
                if (nodeTypeDictionary.TryGetValue(type, out Type nodeType))
                {
                    return (BaseNodeType)Activator.CreateInstance(nodeType, source);
                }
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

        static NodeFactory()
        {
            Layer.Register<AdjustmentLayer, AdjustmentLayerNode>();
            Layer.Register<BitmapLayer, BitmapLayerNode>();
        }
    }
}