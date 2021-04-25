using Fotografix.Drawing;
using Fotografix.Editor.Commands;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingTool<T> : BitmapTool where T : class, IDrawable
    {
        private T drawable;

        public override ToolCursor Cursor => ActiveBitmap != null ? ToolCursor.Crosshair : ToolCursor.Disabled;

        public override void PointerPressed(PointerState p)
        {
            if (ActiveBitmap != null)
            {
                this.drawable = CreateDrawable(Image, p);
                ActiveBitmap.SetDrawingPreview(drawable);
            }
        }

        public override void PointerMoved(PointerState p)
        {
            if (drawable != null)
            {
                UpdateDrawable(drawable, p);
            }
        }

        public override void PointerReleased(PointerState p)
        {
            if (drawable != null)
            {
                ActiveBitmap.SetDrawingPreview(null);
                Image.Dispatch(new DrawCommand(ActiveLayer, drawable));
                this.drawable = null;
            }
        }

        protected abstract T CreateDrawable(Image image, PointerState p);
        protected abstract void UpdateDrawable(T drawable, PointerState p);
    }
}
