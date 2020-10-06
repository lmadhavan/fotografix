using Fotografix.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Tests.UI
{
    public class FakeImageDecoder : IImageDecoder
    {
        public IEnumerable<FileFormat> SupportedFileFormats => Enumerable.Empty<FileFormat>();

        public Task<Image> ReadImageAsync(IFile file)
        {
            BitmapLayer layer = new BitmapLayer(Bitmap.Empty) { Name = file.Name };
            Image image = new Image(layer);
            return Task.FromResult(image);
        }
    }
}
