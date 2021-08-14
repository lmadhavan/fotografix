using System;
using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class CropCommand : SynchronousDocumentCommand
    {
        public override bool CanExecute(Document document, object parameter)
        {
            return parameter is CropCommandArgs args
                && args.Image == document.Image;
        }

        public override void Execute(Document document, object parameter)
        {
            var args = (CropCommandArgs)parameter;

            if (args.Image != document.Image)
            {
                throw new InvalidOperationException();
            }

            args.Image.Crop(args.Rectangle);
        }
    }

    public sealed record CropCommandArgs(Image Image, Rectangle Rectangle);
}
