using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public interface ITool : IPointerEventListener
    {
        object Settings { get; }
        ToolCursor Cursor { get; }

        void LayerActivated(Layer layer);
    }
}
