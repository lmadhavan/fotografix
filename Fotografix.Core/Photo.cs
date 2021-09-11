using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Fotografix
{
    public sealed class Photo
    {
        private readonly StorageFile file;

        public Photo(StorageFile file)
        {
            this.file = file;
        }

        public string Name => file.Name;
        public IRandomAccessStreamReference Content => file;

        public Task<StorageItemThumbnail> GetThumbnailAsync()
        {
            return file.GetThumbnailAsync(ThumbnailMode.SingleItem).AsTask();
        }
    }
}