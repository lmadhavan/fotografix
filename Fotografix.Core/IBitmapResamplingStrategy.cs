using System.Drawing;

namespace Fotografix
{
    public interface IBitmapResamplingStrategy
    {
        Bitmap Resample(Bitmap bitmap, Size newSize);
    }
}
