namespace Fotografix.Editor
{
    public interface IPointerEventListener
    {
        void PointerPressed(IPointerEvent e);
        void PointerMoved(IPointerEvent e);
        void PointerReleased(IPointerEvent e);
    }
}
