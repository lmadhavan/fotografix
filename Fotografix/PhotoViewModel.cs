using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Fotografix
{
    public sealed class PhotoViewModel : NotifyPropertyChangedBase
    {
        private readonly Photo photo;

        public PhotoViewModel(Photo photo)
        {
            this.photo = photo;
            photo.ThumbnailUpdated += (s, e) => RaisePropertyChanged(nameof(Thumbnail));
        }

        public NotifyTaskCompletion<BitmapSource> Thumbnail => new NotifyTaskCompletion<BitmapSource>(LoadThumbnailAsync());

        private async Task<BitmapSource> LoadThumbnailAsync()
        {
            var bitmap = new BitmapImage();
            using (var stream = await photo.GetThumbnailAsync())
            {
                await bitmap.SetSourceAsync(stream);
            }
            return bitmap;
        }

        internal Task<PhotoEditor> CreateEditorAsync()
        {
            return PhotoEditor.CreateAsync(photo);
        }
    }
}
