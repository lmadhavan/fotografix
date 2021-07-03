using Fotografix.Editor.Clipboard;
using Fotografix.Uwp.Codecs;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;

namespace Fotografix.Uwp
{
    public sealed class ClipboardAdapter : IClipboard, IDisposable
    {
        private readonly CoreWindow window;

        public ClipboardAdapter(CoreWindow window)
        {
            this.window = window;
            window.Activated += Window_Activated;
            Clipboard.ContentChanged += Clipboard_ContentChanged;
        }

        public static ClipboardAdapter GetForCurrentThread()
        {
            return new ClipboardAdapter(CoreWindow.GetForCurrentThread());
        }

        public void Dispose()
        {
            Clipboard.ContentChanged -= Clipboard_ContentChanged;
            window.Activated -= Window_Activated;
        }

        public bool HasBitmap => WindowActivated && ClipboardContent.Contains(StandardDataFormats.Bitmap);

        public async Task<Bitmap> GetBitmapAsync()
        {
            var streamRef = await ClipboardContent.GetBitmapAsync();
            using (var stream = await streamRef.OpenReadAsync())
            {
                return await WindowsImageDecoder.ReadBitmapAsync(stream);
            }
        }

        public event EventHandler ContentChanged;

        private bool WindowActivated => window.ActivationMode == CoreWindowActivationMode.ActivatedInForeground;
        private DataPackageView ClipboardContent => Clipboard.GetContent();

        private void Window_Activated(CoreWindow sender, WindowActivatedEventArgs args)
        {
            RaiseContentChanged();
        }

        private void Clipboard_ContentChanged(object sender, object e)
        {
            RaiseContentChanged();
        }

        private void RaiseContentChanged()
        {
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
