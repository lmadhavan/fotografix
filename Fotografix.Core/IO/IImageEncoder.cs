using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.IO
{
    public interface IImageEncoder
    {
        IEnumerable<FileFormat> SupportedFileFormats { get; }
        Task WriteImageAsync(Image image, IFile file, FileFormat fileFormat);
    }
}
