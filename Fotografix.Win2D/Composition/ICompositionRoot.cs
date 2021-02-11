using Fotografix.Editor;
using Microsoft.Graphics.Canvas;

namespace Fotografix.Win2D.Composition
{
    internal interface ICompositionRoot
    {
        ICanvasResourceCreator ResourceCreator { get; }
        Viewport Viewport { get; }

        void Invalidate();
    }
}