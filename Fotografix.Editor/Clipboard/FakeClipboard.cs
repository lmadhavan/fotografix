using System;
using System.Threading.Tasks;

namespace Fotografix.Editor.Clipboard
{
    public sealed class FakeClipboard : IClipboard
    {
        private Bitmap bitmap;

        public bool HasBitmap => bitmap != null;

        public Task<Bitmap> GetBitmapAsync()
        {
            return Task.FromResult(bitmap);
        }

        public void SetBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            SetBitmap(null);
        }

        public event EventHandler ContentChanged;
    }
}
