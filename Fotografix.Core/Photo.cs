using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Fotografix
{
    public sealed class Photo
    {
        private readonly StorageFile content;
        private readonly StorageFileReference sidecar;

        public Photo(StorageFile content, StorageFileReference sidecar)
        {
            this.content = content;
            this.sidecar = sidecar;
        }

        public string Name => content.Name;

        public async Task<IRandomAccessStream> OpenContentAsync()
        {
            return await content.OpenReadAsync();
        }

        public async Task<IRandomAccessStream> GetThumbnailAsync()
        {
            return await content.GetThumbnailAsync(ThumbnailMode.SingleItem);
        }

        internal async Task<PhotoAdjustment> LoadAdjustmentAsync()
        {
            var file = await sidecar.TryGetFileAsync();

            if (file == null)
            {
                return new PhotoAdjustment();
            }

            Debug.WriteLine($"{Name}: Loading saved adjustment from {file.Path}");

            using (var stream = await file.OpenReadAsync())
            using (var reader = new StreamReader(stream.AsStream()))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (PhotoAdjustment)serializer.Deserialize(reader, typeof(PhotoAdjustment));
            }
        }

        internal async Task SaveAdjustmentAsync(PhotoAdjustment adjustment)
        {
            var file = await sidecar.GetOrCreateFileAsync();

            Debug.WriteLine($"{Name}: Saving adjustment to {file.Path}");

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            using (var writer = new StreamWriter(stream.AsStream()))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, adjustment);
            }
        }

        internal Task DeleteAdjustmentAsync()
        {
            Debug.WriteLine($"{Name}: Deleting saved adjustment");
            return sidecar.DeleteAsync();
        }
    }
}