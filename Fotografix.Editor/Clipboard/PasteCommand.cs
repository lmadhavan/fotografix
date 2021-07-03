using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Editor.Clipboard
{
    public sealed class PasteCommand : IObservableDocumentCommand
    {
        private readonly IClipboard clipboard;

        public PasteCommand(IClipboard clipboard)
        {
            this.clipboard = clipboard;
        }

        public event EventHandler CanExecuteChanged
        {
            add => clipboard.ContentChanged += value;
            remove => clipboard.ContentChanged -= value;
        }

        public bool CanExecute(Document document)
        {
            return clipboard.HasBitmap;
        }

        public async Task ExecuteAsync(Document document)
        {
            Image image = document.Image;

            Bitmap bitmap = await clipboard.GetBitmapAsync();
            bitmap.Position = CenterAlign(child: bitmap.Size, parent: image.Size);

            image.Layers.Add(new Layer(bitmap));
        }

        private Point CenterAlign(Size child, Size parent)
        {
            return new(
                (parent.Width - child.Width) / 2,
                (parent.Height - child.Height) / 2
            );
        }
    }
}
