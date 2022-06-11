using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;

namespace Fotografix
{
    public sealed class FakePhoto : IPhoto
    {
        public string Name { get; set; }
        public Size Size { get; set; }

        public Task<CanvasBitmap> LoadBitmapAsync(ICanvasResourceCreator canvasResourceCreator)
        {
            return Task.FromResult<CanvasBitmap>(new CanvasRenderTarget(canvasResourceCreator, (float)Size.Width, (float)Size.Height, 96));
        }

        public Task<bool> HasAdjustmentAsync()
        {
            return Task.FromResult(false);
        }

        public Task<PhotoAdjustment> LoadAdjustmentAsync()
        {
            return Task.FromResult<PhotoAdjustment>(null);
        }

        public Task SaveAdjustmentAsync(PhotoAdjustment adjustment, SoftwareBitmap thumbnail)
        {
            return Task.CompletedTask;
        }

        public Task<PhotoMetadata> GetMetadataAsync()
        {
            return Task.FromResult(PhotoMetadata.Empty);
        }
    }
}
