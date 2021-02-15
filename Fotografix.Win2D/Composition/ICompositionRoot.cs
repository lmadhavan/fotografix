using Microsoft.Graphics.Canvas;

namespace Fotografix.Win2D.Composition
{
    internal interface ICompositionRoot
    {
        ICanvasResourceCreator ResourceCreator { get; }

        void Invalidate();
    }
}