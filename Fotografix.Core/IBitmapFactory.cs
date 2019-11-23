using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Fotografix
{
    public interface IBitmapFactory
    {
        IBitmap CreateBitmap(Size size);
        Task<IBitmap> LoadBitmapAsync(Stream stream);
    }
}
