using System.Drawing;

namespace Fotografix.Win2D
{
    public sealed class Win2DBitmapResamplingStrategy : IBitmapResamplingStrategy
    {
        public Bitmap Resample(Bitmap bitmap, Size newSize)
        {
            return ((Win2DBitmap)bitmap).Scale(newSize);
        }
    }
}
