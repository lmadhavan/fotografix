using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Fotografix
{
    public sealed class Photo
    {
        private const string AdjustmentMetadataKey = "/xmp/{wstr=https://ns.fotografix.org/}:Adjustment";

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
            var file = await sidecar.TryGetFileAsync();

            if (file != null)
            {
                return await file.OpenReadAsync();
            }

            return await content.GetThumbnailAsync(ThumbnailMode.SingleItem);
        }

        public event EventHandler ThumbnailUpdated;

        internal async Task<PhotoAdjustment> LoadAdjustmentAsync()
        {
            var file = await sidecar.TryGetFileAsync();

            if (file == null)
            {
                return new PhotoAdjustment();
            }

            Debug.WriteLine($"{Name}: Loading saved adjustment from {file.Path}");

            using (var stream = await file.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var metadata = await decoder.BitmapProperties.GetPropertiesAsync(new string[] { AdjustmentMetadataKey });

                var serializedAdjustment = metadata[AdjustmentMetadataKey].Value as string;
                return PhotoAdjustment.Deserialize(serializedAdjustment);
            }
        }

        internal async Task SaveAdjustmentAsync(PhotoAdjustment adjustment, SoftwareBitmap thumbnail)
        {
            var file = await sidecar.GetOrCreateFileAsync();

            Debug.WriteLine($"{Name}: Saving adjustment to {file.Path}");

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetSoftwareBitmap(thumbnail);

                var serializedAdjustment = adjustment.Serialize();
                var metadata = new Dictionary<string, BitmapTypedValue>
                {
                    [AdjustmentMetadataKey] = new BitmapTypedValue(serializedAdjustment, PropertyType.String)
                };

                await encoder.BitmapProperties.SetPropertiesAsync(metadata);
                await encoder.FlushAsync();
            }

            ThumbnailUpdated?.Invoke(this, EventArgs.Empty);
        }

        internal async Task DeleteAdjustmentAsync()
        {
            Debug.WriteLine($"{Name}: Deleting saved adjustment");
            await sidecar.DeleteAsync();
            ThumbnailUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}