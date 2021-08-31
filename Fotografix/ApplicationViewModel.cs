using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Fotografix
{
    public sealed class ApplicationViewModel : NotifyPropertyChangedBase
    {
        private NotifyTaskCompletion<IList<PhotoViewModel>> photos;
        private PhotoViewModel selectedPhoto;

        public async void OpenFolder()
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                this.Photos = new NotifyTaskCompletion<IList<PhotoViewModel>>(LoadPhotosAsync(folder));
            }
        }

        public NotifyTaskCompletion<IList<PhotoViewModel>> Photos
        {
            get => photos;
            private set => SetProperty(ref photos, value);
        }

        public PhotoViewModel SelectedPhoto
        {
            get => selectedPhoto;
            set => SetProperty(ref selectedPhoto, value);
        }

        private async Task<IList<PhotoViewModel>> LoadPhotosAsync(StorageFolder folder)
        {
            var photos = await PhotoFolder.GetPhotosAsync(folder);
            return photos.Select(p => new PhotoViewModel(p)).ToList();
        }
    }
}
