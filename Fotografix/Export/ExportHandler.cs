using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Export
{
    public sealed class ExportHandler : IExportHandler
    {
        public StorageFolder DefaultDestinationFolder { get; set; }

        public async Task ExportAsync(IReadOnlyCollection<IExportable> items, bool showDialog, CancellationToken token = default, IProgress<ExportProgress> progress = null)
        {
            var vm = new ExportViewModel(DefaultDestinationFolder, items.Count);

            if (showDialog)
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
                progress?.Report(new ExportProgress { TotalItems = items.Count, CompletedItems = completed });

                var file = await Task.Run(async () => await item.ExportAsync(exportOptions));
                launcherOptions.ItemsToSelect.Add(file);
                completed++;

                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

            await Launcher.LaunchFolderAsync(exportOptions.DestinationFolder, launcherOptions);
        }
    }
}
