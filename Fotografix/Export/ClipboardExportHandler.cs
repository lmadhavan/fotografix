using System.Collections.Generic;
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
            var file = await items.First().ExportAsync(new ExportOptions(ApplicationData.Current.TemporaryFolder));
            
            var dataPackage = new DataPackage();
            dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));
            Clipboard.SetContent(dataPackage);
        }
    }
}
