using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Fotografix
{
    public sealed class PhotoViewModel : NotifyPropertyChangedBase
    {
        private readonly Photo photo;

        public PhotoViewModel(Photo photo)
        {
            this.photo = photo;
        }

        public NotifyTaskCompletion<BitmapSource> Thumbnail => CreateBitmapSource(photo.Thumbnail);
        public NotifyTaskCompletion<BitmapSource> Preview => CreateBitmapSource(photo.Content);

        private NotifyTaskCompletion<BitmapSource> CreateBitmapSource(IRandomAccessStreamReference streamReference)
        {
            return new NotifyTaskCompletion<BitmapSource>(CreateBitmapSourceAsync(streamReference));
        }

        private async Task<BitmapSource> CreateBitmapSourceAsync(IRandomAccessStreamReference streamReference)
        {
            var bitmap = new BitmapImage();
            using (var stream = await streamReference.OpenReadAsync())
            {
                await bitmap.SetSourceAsync(stream);
            }
            return bitmap;
        }
    }
}
