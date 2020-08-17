using Fotografix.Drawing;
using Fotografix.Editor.Drawing;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingTool<T> : ITool, IDrawingSurfaceListener where T : class, IDrawable
    {
        private IDrawingSurface drawingSurface;
        private T drawable;

        public abstract string Name { get; }
        object ITool.Settings => Settings;
        public ToolCursor Cursor => Enabled ? ToolCursor.Crosshair : ToolCursor.Disabled;

        private bool Enabled => drawingSurface != null;

        public void DrawingSurfaceActivated(IDrawingSurface drawingSurface)
        {
            this.drawingSurface = drawingSurface;
        }

        public void PointerPressed(PointerState p)
        {
            if (Enabled)
            {
                this.drawable = CreateDrawable(p);
                drawingSurface.BeginDrawing(drawable);
            }
        }

        public void PointerMoved(PointerState p)
        {
            if (drawable != null)
            {
                UpdateDrawable(drawable, p);
            }
        }

        public void PointerReleased(PointerState p)
        {
            if (drawable != null)
            {
                drawingSurface.EndDrawing(drawable);
                this.drawable = null;
            }
        }
        
        protected abstract object Settings { get; }

        protected abstract T CreateDrawable(PointerState p);
        protected abstract void UpdateDrawable(T drawable, PointerState p);
    }
}
