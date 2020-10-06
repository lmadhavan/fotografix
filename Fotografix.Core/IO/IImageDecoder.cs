using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.IO
{
    public interface IImageDecoder
    {
        IEnumerable<FileFormat> SupportedFileFormats { get; }
        Task<Image> ReadImageAsync(IFile file);
    }
}