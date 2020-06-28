namespace Fotografix.Editor
{
    public interface IPointerEventListener
    {
        void PointerPressed(PointerState p);
        void PointerMoved(PointerState p);
        void PointerReleased(PointerState p);
    }
}
