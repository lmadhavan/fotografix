using Microsoft.Graphics.Canvas;
using System.Drawing;

namespace Fotografix.Win2D
{
    public sealed class Win2DBitmapResamplingStrategy : IBitmapResamplingStrategy
    {
        public Bitmap Resample(Bitmap bitmap, Size newSize)
        {
            if (newSize == Size.Empty)
            {
                return new Bitmap(Size.Empty);
            }

            return new Win2DBitmap(bitmap, CanvasDevice.GetSharedDevice()).Scale(newSize).Source;
        }
    }
}
