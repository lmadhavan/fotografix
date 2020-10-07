using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.IO
{
    public interface IImageEncoder
    {
        IEnumerable<FileFormat> SupportedFileFormats { get; }
        Task WriteImageAsync(Image image, IFile file, FileFormat fileFormat);
    }

    public static class ImageEncoderExtensions
    {
        public static Task WriteImageAsync(this IImageEncoder encoder, Image image, IFile file)
        {
            FileFormat fileFormat = encoder.SupportedFileFormats.First(f => f.Matches(file));
            return encoder.WriteImageAsync(image, file, fileFormat);
        }
    }
}
