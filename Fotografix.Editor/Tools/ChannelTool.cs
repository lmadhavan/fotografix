using System.ComponentModel;

namespace Fotografix.Editor.Tools
{
    public abstract class ChannelTool : ITool
    {
        public abstract string Name { get; }
        public abstract ToolCursor Cursor { get; }

        protected Image Image { get; private set; }
        protected Channel ActiveChannel { get; private set; }

        public void Activated(Image image)
        {
            this.Image = image;
            UpdateActiveChannel();
            image.UserPropertyChanged += Image_PropertyChanged;
        }

        public void Deactivated()
        {
            Image.UserPropertyChanged -= Image_PropertyChanged;
            this.ActiveChannel = null;
            this.Image = null;
        }

        public abstract void PointerPressed(PointerState p);
        public abstract void PointerMoved(PointerState p);
        public abstract void PointerReleased(PointerState p);

        private void Image_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == EditorProperties.ActiveLayer)
            {
                UpdateActiveChannel();
            }
        }

        private void UpdateActiveChannel()
        {
            Layer layer = Image.GetActiveLayer();
            this.ActiveChannel = layer.ContentChannel;
        }
    }
}
