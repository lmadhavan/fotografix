using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Fotografix.Export
{
    public sealed class ShareExportHandler : IExportHandler
    {
        private StorageFile file;

        public ShareExportHandler()
        {
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        public async Task ExportAsync(IReadOnlyCollection<IExportable> items)
        {
            this.file = await items.First().ExportAsync(new ExportOptions(ApplicationData.Current.TemporaryFolder));
            DataTransferManager.ShowShareUI();
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (file == null)
            {
                return;
            }

            var data = args.Request.Data;
            data.Properties.Title = file.DisplayName;
            data.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));
            this.file = null;
        }
    }
}
