using System.Drawing;

namespace Fotografix
{
    public interface IBitmapFactory
    {
        Bitmap CreateBitmap(Size size);
    }
}
