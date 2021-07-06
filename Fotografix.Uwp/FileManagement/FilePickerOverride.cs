using Fotografix.Editor;
using Fotografix.Editor.FileManagement;
using Fotografix.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Uwp.FileManagement
{
    public sealed class FilePickerOverride : IFilePicker
    {
        private readonly IFilePicker filePicker;
        private IEnumerable<IFile> openOverride;
        private IFile saveOverride;

        public FilePickerOverride(IFilePicker filePicker)
        {
            this.filePicker = filePicker;
        }

        public IDisposable OverrideOpenFiles(IEnumerable<IFile> files)
        {
            this.openOverride = files;
            return new DisposableAction(() => this.openOverride = null);
        }

        public IDisposable OverrideSaveFile(IFile file)
        {
            this.saveOverride = file;
            return new DisposableAction(() => this.saveOverride = null);
        }

        public Task<IEnumerable<IFile>> PickOpenFilesAsync(IEnumerable<FileFormat> fileFormats)
        {
            if (openOverride != null)
            {
                return Task.FromResult(openOverride);
            }

            return filePicker.PickOpenFilesAsync(fileFormats);
        }

        public Task<IFile> PickSaveFileAsync(IEnumerable<FileFormat> fileFormats)
        {
            if (saveOverride != null)
            {
                return Task.FromResult(saveOverride);
            }

            return filePicker.PickSaveFileAsync(fileFormats);
        }
    }
}
