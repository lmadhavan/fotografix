using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;

namespace Fotografix.Win2D
{
    public sealed class Win2DCompositorSettings
    {
        public ICanvasResourceCreator ResourceCreator { get; set; } = CanvasDevice.GetSharedDevice();
        public int TransparencyGridSize { get; set; } = 0;
        public bool InteractiveMode { get; set; } = false;

        internal NodeFactory CreateNodeFactory()
        {
            return new NodeFactory(ResourceCreator, TransparencyGridSize, InteractiveMode);
        }
    }
}
