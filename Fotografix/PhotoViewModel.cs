using System.Threading.Tasks;

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

        public NotifyTaskCompletion<ThumbnailViewModel> Thumbnail => new NotifyTaskCompletion<ThumbnailViewModel>(ThumbnailViewModel.CreateAsync(photo));

        internal Task<PhotoEditor> CreateEditorAsync()
        {
            return PhotoEditor.CreateAsync(photo);
        }
    }
}
