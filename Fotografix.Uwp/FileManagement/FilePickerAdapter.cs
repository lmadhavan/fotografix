using Fotografix.Editor;
using Fotografix.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Uwp.FileManagement
{
    public sealed class FilePickerAdapter : IFilePicker
    {
        public async Task<IFile> PickSaveFileAsync(IEnumerable<FileFormat> fileFormats)
        {
            var picker = FilePickerFactory.CreateFileSavePicker(fileFormats);
            var storageFile = await picker.PickSaveFileAsync();
            return storageFile == null ? null : new StorageFileAdapter(storageFile);
        }
    }
}
