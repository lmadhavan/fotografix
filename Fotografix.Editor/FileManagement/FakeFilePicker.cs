using Fotografix.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    public sealed class FakeFilePicker : IFilePicker
    {
        public IEnumerable<IFile> OpenFilesResult { get; set; } = Enumerable.Empty<IFile>();
        public IFile SaveFileResult { get; set; }

        public int OpenCount { get; private set; }
        public int SaveCount { get; private set; }

        public Task<IEnumerable<IFile>> PickOpenFilesAsync(IEnumerable<FileFormat> fileFormats)
        {
            OpenCount++;
            return Task.FromResult(OpenFilesResult);
        }

        public Task<IFile> PickSaveFileAsync(IEnumerable<FileFormat> fileFormats)
        {
            SaveCount++;
            return Task.FromResult(SaveFileResult);
        }
    }
}
