using Fotografix.Drawing;
using Fotografix.Editor.Commands;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingTool<T> : ChannelTool where T : class, IDrawable
    {
        private T drawable;

        private bool CanDraw => ActiveChannel?.CanDraw ?? false;
        public override ToolCursor Cursor => CanDraw ? ToolCursor.Crosshair : ToolCursor.Disabled;

        public override void PointerPressed(PointerState p)
        {
            if (CanDraw)
            {
                this.drawable = CreateDrawable(Image, p);
                ActiveChannel.SetDrawingPreview(drawable);
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
                ActiveChannel.SetDrawingPreview(null);
                Image.Dispatch(new DrawCommand(ActiveChannel, drawable));
                this.drawable = null;
            }
        }

        protected abstract T CreateDrawable(Image image, PointerState p);
        protected abstract void UpdateDrawable(T drawable, PointerState p);
    }
}
