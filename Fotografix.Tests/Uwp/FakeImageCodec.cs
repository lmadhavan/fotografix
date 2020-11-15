using Fotografix.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Tests.Uwp
{
    public class FakeImageCodec : IImageDecoder, IImageEncoder
    {
        public static readonly FakeImageCodec Instance = new FakeImageCodec();

        public IEnumerable<FileFormat> SupportedFileFormats => Enumerable.Empty<FileFormat>();

        public Task<Image> ReadImageAsync(IFile file)
        {
            BitmapLayer layer = new BitmapLayer { Name = file.Name };
            Image image = new Image(layer);
            return Task.FromResult(image);
        }

        public Task WriteImageAsync(Image image, IFile file, FileFormat fileFormat)
        {
            return Task.CompletedTask;
        }
    }
}
