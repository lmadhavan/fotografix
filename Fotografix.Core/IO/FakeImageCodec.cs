using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.IO
{
    public sealed class FakeImageCodec : IImageEncoder, IImageDecoder
    {
        public IEnumerable<FileFormat> SupportedFileFormats { get; set; } = Enumerable.Empty<FileFormat>();
        public IDictionary<IFile, Image> SavedImages { get; } = new Dictionary<IFile, Image>();

        public int ReadCount { get; private set; }
        public int WriteCount { get; private set; }

        public Task<Image> ReadImageAsync(IFile file)
        {
            ReadCount++;
            return Task.FromResult(SavedImages[file]);
        }

        public Task WriteImageAsync(Image image, IFile file, FileFormat fileFormat)
        {
            SavedImages[file] = image;
            WriteCount++;
            return Task.CompletedTask;
        }
    }
}
