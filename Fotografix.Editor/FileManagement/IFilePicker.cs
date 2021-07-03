using Fotografix.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    public interface IFilePicker
    {
        Task<IFile> PickSaveFileAsync(IEnumerable<FileFormat> fileFormats);
    }
}
