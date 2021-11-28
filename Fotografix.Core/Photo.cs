using Fotografix.Xmp;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Fotografix
{
    public sealed class Photo : IPhoto
    {
        private static readonly XmpProperty AdjustmentMetadataProperty = new XmpProperty("https://ns.fotografix.org/", "Adjustment");

        private readonly StorageFile content;
        private readonly StorageFileReference sidecar;

        public Photo(StorageFile content, StorageFileReference sidecar)
        {
            this.content = content;
            this.sidecar = sidecar;
        }

        public string Name => content.Name;
        public List<Photo> LinkedPhotos { get; } = new List<Photo>();
        public IEnumerable<IStorageItem> LinkedStorageItems => Enumerable.Concat(new IStorageItem[] { content }, LinkedPhotos.Select(p => p.content));

        public async Task<CanvasBitmap> LoadBitmapAsync(ICanvasResourceCreator canvasResourceCreator)
        {
            using (var stream = await content.OpenReadAsync())
            {
                return await CanvasBitmap.LoadAsync(canvasResourceCreator, stream);
            }
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

        public event EventHandler Changed;

        public async Task<bool> HasAdjustmentAsync()
        {
            return (await sidecar.TryGetFileAsync()) != null;
        }

        public async Task<PhotoAdjustment> LoadAdjustmentAsync()
        {
            var file = await sidecar.TryGetFileAsync();

            if (file == null)
            {
                return null;
            }

            Debug.WriteLine($"{Name}: Loading saved adjustment from {file.Path}");

            using (var stream = await file.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var serializedAdjustment = await decoder.BitmapProperties.GetXmpPropertyAsync(AdjustmentMetadataProperty);
                return PhotoAdjustment.Deserialize(serializedAdjustment.Value as string);
            }
        }

        public async Task SaveAdjustmentAsync(PhotoAdjustment adjustment, SoftwareBitmap thumbnail)
        {
            var serializedAdjustment = adjustment.Serialize();
            if (serializedAdjustment == "")
            {
                await DeleteAdjustmentAsync();
                return;
            }

            var file = await sidecar.GetOrCreateFileAsync();

            Debug.WriteLine($"{Name}: Saving adjustment to {file.Path}");

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetSoftwareBitmap(thumbnail);

                await encoder.BitmapProperties.SetXmpPropertyAsync(AdjustmentMetadataProperty, serializedAdjustment);
                await encoder.FlushAsync();
            }

            Changed?.Invoke(this, EventArgs.Empty);
        }

        private async Task DeleteAdjustmentAsync()
        {
            Debug.WriteLine($"{Name}: Deleting saved adjustment");
            await sidecar.DeleteAsync();
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}