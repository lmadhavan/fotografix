using Fotografix.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    internal abstract class BatchImageProcessor<T>
    {
        private readonly IImageDecoder imageDecoder;
        private readonly IFilePicker filePicker;

        protected BatchImageProcessor(IImageDecoder imageDecoder, IFilePicker filePicker)
        {
            this.imageDecoder = imageDecoder;
            this.filePicker = filePicker;
        }

        public async Task ProcessBatchAsync(T context, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            var files = await GetFiles(parameter);

            for (int i = 0; i < files.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var file = files[i];
                progress?.Report(new(GetProgressDescription(file), i, files.Count));
                await ProcessAsync(context, file);
            }
        }

        protected Task<Image> ReadImageAsync(IFile file)
        {
            return imageDecoder.ReadImageAsync(file);
        }

        protected abstract string GetProgressDescription(IFile file);
        protected abstract Task ProcessAsync(T context, IFile file);

        private async Task<List<IFile>> GetFiles(object parameter)
        {
            if (parameter is IFile file)
            {
                return new List<IFile> { file };
            }

            if (parameter is IEnumerable<IFile> files)
            {
                return files.ToList();
            }

            files = await filePicker.PickOpenFilesAsync(imageDecoder.SupportedFileFormats);
            return files.ToList();
        }
    }
}
