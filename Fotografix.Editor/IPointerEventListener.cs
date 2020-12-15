using Fotografix.Editor.Tools;

namespace Fotografix.Editor
{
    public interface IPointerEventListener
    {
        ToolCursor Cursor { get; }

        void PointerPressed(PointerState p);
        void PointerMoved(PointerState p);
        void PointerReleased(PointerState p);
    }
}
