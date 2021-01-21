using Fotografix.Drawing;
using Fotografix.Editor.Drawing;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingTool<T> : ITool where T : class, IDrawable
    {
        private Image image;
        private BitmapLayer activeLayer;
        private T drawable;

        public abstract string Name { get; }
        public ToolCursor Cursor => Enabled ? ToolCursor.Crosshair : ToolCursor.Disabled;

        private bool Enabled => activeLayer != null;

        public void Activated(Image image)
        {
            this.image = image;
            UpdateActiveLayer();
            image.UserPropertyChanged += Image_UserPropertyChanged;
        }

        public void Deactivated()
        {
            image.UserPropertyChanged -= Image_UserPropertyChanged;
            this.activeLayer = null;
            this.image = null;
        }

        public void PointerPressed(PointerState p)
        {
            if (Enabled)
            {
                this.drawable = CreateDrawable(p);
                activeLayer.SetDrawingPreview(drawable);
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
                activeLayer.SetDrawingPreview(null);
                image.Dispatch(new DrawCommand(activeLayer.Bitmap, drawable));
                this.drawable = null;
            }
        }

        private void Image_UserPropertyChanged(object sender, UserPropertyChangedEventArgs e)
        {
            if (e.Key == EditorProperties.ActiveLayerProperty)
            {
                UpdateActiveLayer();
            }
        }

        private void UpdateActiveLayer()
        {
            this.activeLayer = image.GetActiveLayer() as BitmapLayer;
        }

        protected abstract T CreateDrawable(PointerState p);
        protected abstract void UpdateDrawable(T drawable, PointerState p);
    }
}
