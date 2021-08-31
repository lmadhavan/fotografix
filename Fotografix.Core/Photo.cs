using Windows.Storage.BulkAccess;
using Windows.Storage.Streams;

namespace Fotografix
{
    public sealed class Photo
    {
        private readonly FileInformation file;

        public Photo(FileInformation file)
        {
            this.file = file;
        }

        public string Name => file.Name;
        public IRandomAccessStreamReference Thumbnail => RandomAccessStreamReference.CreateFromStream(file.Thumbnail);
        public IRandomAccessStreamReference Content => file;
    }
}