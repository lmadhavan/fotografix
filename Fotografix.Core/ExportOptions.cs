using System;
using Windows.Storage;

namespace Fotografix
{
    public sealed class ExportOptions
    {
        public ExportOptions(StorageFolder destinationFolder)
        {
            this.DestinationFolder = destinationFolder ?? throw new ArgumentNullException(nameof(destinationFolder));
        }

        public StorageFolder DestinationFolder { get; }
        public int? MaxDimension { get; set; }
        public float Quality { get; set; } = 0.9f;
    }
}
