using System;
using System.Threading.Tasks;

namespace Fotografix.Editor.Clipboard
{
    public interface IClipboard
    {
        bool HasBitmap { get; }
        Task<Bitmap> GetBitmapAsync();

        event EventHandler ContentChanged;
    }
}
