using Fotografix.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    public interface IFilePicker
    {
        Task<IEnumerable<IFile>> PickOpenFilesAsync(IEnumerable<FileFormat> fileFormats);
        Task<IFile> PickSaveFileAsync(IEnumerable<FileFormat> fileFormats);
    }
}
