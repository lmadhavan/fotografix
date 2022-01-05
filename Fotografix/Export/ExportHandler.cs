using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Export
{
    public sealed class ExportHandler : IExportHandler
    {
        public StorageFolder DefaultDestinationFolder { get; set; }

        public async Task ExportAsync(IEnumerable<IExportable> items, bool showDialog)
        {
            var vm = new ExportViewModel(DefaultDestinationFolder);

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

            foreach (var item in items)
            {
                var file = await item.ExportAsync(exportOptions);
                launcherOptions.ItemsToSelect.Add(file);
            }

            await Launcher.LaunchFolderAsync(exportOptions.DestinationFolder, launcherOptions);
        }
    }
}
