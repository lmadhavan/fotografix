using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Fotografix
{
    public sealed class ThumbnailViewModel
    {
        public const int ThumbnailHeight = 96;

        public ThumbnailViewModel(BitmapSource source)
        {
            this.Source = source;
        }

        public BitmapSource Source { get; }
        public int Width => Source.PixelWidth * ThumbnailHeight / Source.PixelHeight;
        public int Height => ThumbnailHeight;

        public static async Task<ThumbnailViewModel> CreateAsync(Photo photo)
        {
            var bitmap = new BitmapImage();

            using (var stream = await photo.GetThumbnailAsync())
            {
                await bitmap.SetSourceAsync(stream);
            }

            return new ThumbnailViewModel(bitmap);
        }
    }
}
