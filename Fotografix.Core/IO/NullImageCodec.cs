using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.IO
{
    public sealed class NullImageCodec : IImageEncoder, IImageDecoder
    {
        public static readonly NullImageCodec Instance = new NullImageCodec();

        public IEnumerable<FileFormat> SupportedFileFormats => Enumerable.Empty<FileFormat>();

        public Task<Image> ReadImageAsync(IFile file)
        {
            throw new NotSupportedException();
        }

        public Task WriteImageAsync(Image image, IFile file, FileFormat fileFormat)
        {
            throw new NotSupportedException();
        }
    }
}
