using System.ComponentModel;

namespace Fotografix.Editor.Tools
{
    public abstract class ChannelTool : ITool
    {
        public abstract string Name { get; }
        public abstract ToolCursor Cursor { get; }

        protected Document Document { get; private set; }
        protected Channel ActiveChannel { get; private set; }

        public void Activated(Document document)
        {
            this.Document = document;
            UpdateActiveChannel();
            Document.PropertyChanged += Document_PropertyChanged;
        }

        public void Deactivated()
        {
            Document.PropertyChanged -= Document_PropertyChanged;
            this.ActiveChannel = null;
            this.Document = null;
        }

        public abstract void PointerPressed(PointerState p);
        public abstract void PointerMoved(PointerState p);
        public abstract void PointerReleased(PointerState p);

        private void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Document.ActiveLayer))
            {
                UpdateActiveChannel();
            }
        }

        private void UpdateActiveChannel()
        {
            this.ActiveChannel = Document.ActiveLayer.ContentChannel;
        }
    }
}
