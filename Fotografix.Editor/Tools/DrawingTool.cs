using Fotografix.Drawing;
using Fotografix.Editor.Commands;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingTool<T> : ChannelTool where T : class, IDrawable
    {
        private T drawable;
        private IDrawable clippedDrawable;

        private bool CanDraw => ActiveChannel?.CanDraw ?? false;
        public override ToolCursor Cursor => CanDraw ? ToolCursor.Crosshair : ToolCursor.Disabled;

        public override void PointerPressed(PointerState p)
        {
            if (CanDraw)
            {
                this.drawable = CreateDrawable(Image, p);
                this.clippedDrawable = ClippedDrawable.Create(drawable, Image.Selection);
                ActiveChannel.SetDrawingPreview(clippedDrawable);
            }
        }

        public override void PointerMoved(PointerState p)
        {
            if (drawable != null)
            {
                UpdateDrawable(drawable, p);
            }
        }

        public async override void PointerReleased(PointerState p)
        {
            if (drawable != null)
            {
                ActiveChannel.SetDrawingPreview(null);
                await Image.DispatchAsync(new DrawCommand(ActiveChannel, clippedDrawable));
                this.drawable = null;
                this.clippedDrawable = null;
            }
        }

        protected abstract T CreateDrawable(Image image, PointerState p);
        protected abstract void UpdateDrawable(T drawable, PointerState p);
    }
}
