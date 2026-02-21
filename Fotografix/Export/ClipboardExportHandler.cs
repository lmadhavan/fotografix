using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Fotografix.Export
{
    public sealed class ClipboardExportHandler : IExportHandler
    {
        public async Task ExportAsync(IReadOnlyCollection<IExportable> items)
        {
            var item = items.First();

            try
            {
                var file = await item.ExportAsync(new ExportOptions(ApplicationData.Current.TemporaryFolder));

                var dataPackage = new DataPackage();
                dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));
                Clipboard.SetContent(dataPackage);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error copying {item.Name} to clipboard: {e.Message}");
                Logger.LogEvent("CopyToClipboardError");
            }
        }
    }
}
