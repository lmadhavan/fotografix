using System.Drawing;

namespace Fotografix.Editor
{
    public interface IBitmapResamplingStrategy
    {
        Bitmap Resample(Bitmap bitmap, Size newSize);
    }
}