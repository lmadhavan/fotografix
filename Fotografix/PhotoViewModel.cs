using Microsoft.Graphics.Canvas;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;

namespace Fotografix
{
    public sealed class PhotoViewModel : NotifyPropertyChangedBase
    {
        private readonly Photo photo;

        public PhotoViewModel(Photo photo)
        {
            this.photo = photo;
            photo.Changed += (s, e) => RaisePropertyChanged("");
        }

        public string Name => photo.Name;
        public NotifyTaskCompletion<ThumbnailViewModel> Thumbnail => new NotifyTaskCompletion<ThumbnailViewModel>(ThumbnailViewModel.CreateAsync(photo));
        public NotifyTaskCompletion<bool> IsEdited => new NotifyTaskCompletion<bool>(photo.HasAdjustmentAsync());

        public async void ShowInFileExplorer()
        {
            var linkedStorageItems = photo.LinkedStorageItems;
            var folderPath = Path.GetDirectoryName(linkedStorageItems.First().Path);

            var options = new FolderLauncherOptions();
            foreach (var item in linkedStorageItems)
            {
                options.ItemsToSelect.Add(item);
            }

            await Launcher.LaunchFolderPathAsync(folderPath, options);
        }

        internal Task<PhotoEditor> CreateEditorAsync(ICanvasResourceCreatorWithDpi canvasResourceCreator)
        {
            return PhotoEditor.CreateAsync(photo, canvasResourceCreator);
        }
    }
}
