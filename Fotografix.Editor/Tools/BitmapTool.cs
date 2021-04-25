using System.ComponentModel;

namespace Fotografix.Editor.Tools
{
    public abstract class BitmapTool : ITool
    {
        public abstract string Name { get; }
        public abstract ToolCursor Cursor { get; }

        protected Image Image { get; private set; }
        protected Layer ActiveLayer { get; private set; }
        protected Bitmap ActiveBitmap { get; private set; }

        public void Activated(Image image)
        {
            this.Image = image;
            UpdateActiveLayer();
            image.UserPropertyChanged += Image_PropertyChanged;
        }

        public void Deactivated()
        {
            Image.UserPropertyChanged -= Image_PropertyChanged;
            this.ActiveLayer = null;
            this.ActiveBitmap = null;
            this.Image = null;
        }

        public abstract void PointerPressed(PointerState p);
        public abstract void PointerMoved(PointerState p);
        public abstract void PointerReleased(PointerState p);

        private void Image_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == EditorProperties.ActiveLayer)
            {
                UpdateActiveLayer();
            }
        }

        private void UpdateActiveLayer()
        {
            this.ActiveLayer = Image.GetActiveLayer();
            this.ActiveBitmap = ActiveLayer.Content as Bitmap;
        }
    }
}
