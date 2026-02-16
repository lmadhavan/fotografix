using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Export
{
    public sealed class FileExportHandler : IExportHandler
    {
        public FileExportHandler(StorageFolder destinationFolder)
        {
            this.DestinationFolder = destinationFolder;
        }

        public StorageFolder DestinationFolder { get; set; }
        public bool ShowDialog { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public IProgress<ExportProgress> Progress { get; set; }

        public async Task ExportAsync(IReadOnlyCollection<IExportable> items)
        {
            var vm = new ExportViewModel(DestinationFolder, items.Count);

            if (ShowDialog)
            {
                var dialog = new ExportDialog(vm);
                if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                {
                    // cancelled by user
                    return;
                }
            }

            var exportOptions = await vm.CreateExportOptionsAsync();
            var launcherOptions = new FolderLauncherOptions();
            int completed = 0;

            foreach (var item in items)
            {
                Progress?.Report(new ExportProgress { TotalItems = items.Count, CompletedItems = completed });

                var file = await Task.Run(async () => await item.ExportAsync(exportOptions));
                launcherOptions.ItemsToSelect.Add(file);
                completed++;

                if (CancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            await Launcher.LaunchFolderAsync(exportOptions.DestinationFolder, launcherOptions);
        }
    }
}
