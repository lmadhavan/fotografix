using Fotografix.Editor;
using Fotografix.IO;
using Fotografix.Win2D;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.UI
{
    public sealed class ImageEditorFactory
    {
        private readonly IImageDecoder imageDecoder = new WindowsImageDecoder();
        private readonly IImageEncoder imageEncoder = new WindowsImageEncoder(new Win2DImageRenderer());

        public IEnumerable<FileFormat> SupportedOpenFormats => imageDecoder.SupportedFileFormats;

        public ImageEditor CreateNewImage(Viewport viewport, Size size)
        {
            BitmapLayer layer = BitmapLayerFactory.CreateBitmapLayer(id: 1);
            Image image = new Image(size);
            image.Layers.Add(layer);

            return CreateEditor(viewport, image);
        }

        public async Task<ImageEditor> OpenImageAsync(Viewport viewport, IFile file)
        {
            Image image = await imageDecoder.ReadImageAsync(file);
            return CreateEditor(viewport, image);
        }

        private ImageEditor CreateEditor(Viewport viewport, Image image)
        {
            return new ImageEditor(image, viewport)
            {
                ImageDecoder = imageDecoder,
                ImageEncoder = imageEncoder
            };
        }
    }
}
