namespace Fotografix.Editor.Tools
{
    public interface ITool
    {
        string Name { get; }
        object Settings { get; }
        ToolCursor Cursor { get; }

        void PointerPressed(PointerState p);
        void PointerMoved(PointerState p);
        void PointerReleased(PointerState p);
    }
}
