using Fotografix.Drawing;
using Fotografix.Editor.Commands;
using System.ComponentModel;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingTool<T> : ITool where T : class, IDrawable
    {
        private Image image;
        private Layer activeLayer;
        private Bitmap activeBitmap;
        private T drawable;

        public abstract string Name { get; }
        public ToolCursor Cursor => Enabled ? ToolCursor.Crosshair : ToolCursor.Disabled;

        private bool Enabled => activeBitmap != null;

        public void Activated(Image image)
        {
            this.image = image;
            UpdateActiveLayer();
            image.UserPropertyChanged += Image_PropertyChanged;
        }

        public void Deactivated()
        {
            image.UserPropertyChanged -= Image_PropertyChanged;
            this.activeLayer = null;
            this.activeBitmap = null;
            this.image = null;
        }

        public void PointerPressed(PointerState p)
        {
            if (Enabled)
            {
                this.drawable = CreateDrawable(image, p);
                activeBitmap.SetDrawingPreview(drawable);
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
                activeBitmap.SetDrawingPreview(null);
                image.Dispatch(new DrawCommand(activeLayer, drawable));
                this.drawable = null;
            }
        }

        private void Image_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == EditorProperties.ActiveLayer)
            {
                UpdateActiveLayer();
            }
        }

        private void UpdateActiveLayer()
        {
            this.activeLayer = image.GetActiveLayer();
            this.activeBitmap = activeLayer.Content as Bitmap;
        }

        protected abstract T CreateDrawable(Image image, PointerState p);
        protected abstract void UpdateDrawable(T drawable, PointerState p);
    }
}
