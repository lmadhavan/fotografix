using Windows.Storage;

namespace Fotografix
{
    public sealed class ExportOptions
    {
        public ExportOptions(StorageFolder destinationFolder)
        {
            this.DestinationFolder = destinationFolder;
        }

        public StorageFolder DestinationFolder { get; }
    }
}
