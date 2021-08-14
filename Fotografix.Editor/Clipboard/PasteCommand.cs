using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor.Clipboard
{
    public sealed class PasteCommand : DocumentCommand
    {
        private readonly IClipboard clipboard;

        public PasteCommand(IClipboard clipboard)
        {
            this.clipboard = clipboard;
        }

        public override bool CanExecute(Document document, object parameter)
        {
            return clipboard.HasBitmap;
        }

        public async override Task ExecuteAsync(Document document, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
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
