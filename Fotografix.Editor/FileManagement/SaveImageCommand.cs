﻿using Fotografix.IO;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    public sealed class SaveImageCommand : DocumentCommand
    {
        private readonly IImageEncoder imageEncoder;
        private readonly IFilePicker filePicker;

        public SaveImageCommand(IImageEncoder imageEncoder, IFilePicker filePicker)
        {
            this.imageEncoder = imageEncoder;
            this.filePicker = filePicker;
        }

        public SaveCommandMode Mode { get; set; } = SaveCommandMode.Save;

        public async override Task ExecuteAsync(Document document, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            IFile file = null;

            if (Mode == SaveCommandMode.Save)
            {
                file = document.File;
            }

            if (file == null || FindFileFormat(file) == null)
            {
                file = await filePicker.PickSaveFileAsync(imageEncoder.SupportedFileFormats);
            }

            if (file == null)
            {
                return;
            }

            await imageEncoder.WriteImageAsync(document.Image, file, FindFileFormat(file));
            document.File = file;
            document.IsDirty = false;
        }

        private FileFormat FindFileFormat(IFile file)
        {
            return imageEncoder.SupportedFileFormats.FirstOrDefault(f => f.Matches(file));
        }
    }

    public enum SaveCommandMode
    {
        Save,
        SaveAs
    }
}
