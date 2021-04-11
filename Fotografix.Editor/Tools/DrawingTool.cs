using Fotografix.Drawing;
using Fotografix.Editor.Commands;
using System.ComponentModel;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingTool<T> : ITool where T : class, IDrawable
    {
        private Image image;
        private Layer activeBitmapLayer;
        private T drawable;

        public abstract string Name { get; }
        public ToolCursor Cursor => Enabled ? ToolCursor.Crosshair : ToolCursor.Disabled;

        private bool Enabled => activeBitmapLayer != null;

        public void Activated(Image image)
        {
            this.image = image;
            UpdateActiveLayer();
            image.UserPropertyChanged += Image_PropertyChanged;
        }

        public void Deactivated()
        {
            image.UserPropertyChanged -= Image_PropertyChanged;
            this.activeBitmapLayer = null;
            this.image = null;
        }

        public void PointerPressed(PointerState p)
        {
            if (Enabled)
            {
                this.drawable = CreateDrawable(image, p);
                activeBitmapLayer.SetDrawingPreview(drawable);
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
                activeBitmapLayer.SetDrawingPreview(null);
                image.Dispatch(new DrawCommand(activeBitmapLayer, drawable));
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
            Layer activeLayer = image.GetActiveLayer();
            this.activeBitmapLayer = activeLayer.Content is Bitmap ? activeLayer : null;
        }

        protected abstract T CreateDrawable(Image image, PointerState p);
        protected abstract void UpdateDrawable(T drawable, PointerState p);
    }
}
