using Fotografix.Drawing;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingTool<T> : ChannelTool where T : class, IDrawable
    {
        private readonly IDocumentCommand drawCommand;

        private T drawable;
        private IDrawable clippedDrawable;

        protected DrawingTool(IDocumentCommand drawCommand)
        {
            this.drawCommand = drawCommand;
        }

        private bool CanDraw => ActiveChannel?.CanDraw ?? false;
        public override ToolCursor Cursor => CanDraw ? ToolCursor.Crosshair : ToolCursor.Disabled;

        public override void PointerPressed(PointerState p)
        {
            if (CanDraw)
            {
                this.drawable = CreateDrawable(Document.Image, p);
                this.clippedDrawable = ClippedDrawable.Create(drawable, Document.Image.Selection);
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
                await drawCommand.ExecuteAsync(Document);
                ActiveChannel.SetDrawingPreview(null);
                this.drawable = null;
                this.clippedDrawable = null;
            }
        }

        protected abstract T CreateDrawable(Image image, PointerState p);
        protected abstract void UpdateDrawable(T drawable, PointerState p);
    }
}
