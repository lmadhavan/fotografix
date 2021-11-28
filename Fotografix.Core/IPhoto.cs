using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace Fotografix
{
    public interface IPhoto
    {
        string Name { get; }
        Task<CanvasBitmap> LoadBitmapAsync(ICanvasResourceCreator canvasResourceCreator);

        Task<bool> HasAdjustmentAsync();
        Task<PhotoAdjustment> LoadAdjustmentAsync();
        Task SaveAdjustmentAsync(PhotoAdjustment adjustment, SoftwareBitmap thumbnail);
    }
}