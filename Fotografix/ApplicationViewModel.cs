using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix
{
    public sealed class ApplicationViewModel : NotifyPropertyChangedBase
    {
        private readonly ISidecarStrategy sidecarStrategy;

        private string folderName;
        private NotifyTaskCompletion<IList<PhotoViewModel>> photos;
        private PhotoViewModel selectedPhoto;
        private NotifyTaskCompletion<EditorViewModel> editor;

        public ApplicationViewModel(ISidecarStrategy sidecarStrategy)
        {
            this.sidecarStrategy = sidecarStrategy;
        }

        public Task DisposeAsync()
        {
            return SaveAsync(dispose: true);
        }

        public ICanvasResourceCreatorWithDpi CanvasResourceCreator { get; set; }

        public string FolderName
        {
            get => folderName;
            private set => SetProperty(ref folderName, value);
        }

        public NotifyTaskCompletion<IList<PhotoViewModel>> Photos
        {
            get => photos;

            private set
            {
                if (SetProperty(ref photos, value))
                {
                    this.SelectedPhoto = null;
                }
            }
        }

        public PhotoViewModel SelectedPhoto
        {
            get => selectedPhoto;

            set
            {
                if (SetProperty(ref selectedPhoto, value))
                {
                    this.Editor = new NotifyTaskCompletion<EditorViewModel>(LoadEditorAsync());
                }
            }
        }

        public NotifyTaskCompletion<EditorViewModel> Editor
        {
            get => editor;
            private set => SetProperty(ref editor, value);
        }

        public event EventHandler<EditorViewModel> EditorLoaded;

        public void OpenFolder(StorageFolder folder)
        {
            this.FolderName = folder.DisplayName;
            this.Photos = new NotifyTaskCompletion<IList<PhotoViewModel>>(LoadPhotosAsync(folder));
        }

        public Task SaveAsync()
        {
            return SaveAsync(dispose: false);
        }

        private async Task<IList<PhotoViewModel>> LoadPhotosAsync(StorageFolder folder)
        {
            Debug.WriteLine($"Opening {folder.Path}");

            var sidecarFolder = await sidecarStrategy.GetSidecarFolderAsync(folder);
            var photoFolder = new PhotoFolder(folder, sidecarFolder);
            var photos = await photoFolder.GetPhotosAsync();

            Debug.WriteLine($"Loaded {photos.Count} photos from {folder.Name}");

            return photos.Select(p => new PhotoViewModel(p)).ToList();
        }

        private async Task<EditorViewModel> LoadEditorAsync()
        {
            await SaveAsync(dispose: true);

            if (selectedPhoto == null)
            {
                return null;
            }

            var editor = await selectedPhoto.CreateEditorAsync(CanvasResourceCreator);
            var vm = new EditorViewModel(editor, CanvasResourceCreator);
            EditorLoaded?.Invoke(this, vm);
            return vm;
        }

        private async Task SaveAsync(bool dispose)
        {
            if (editor != null)
            {
                try
                {
                    var vm = await editor.Task;

                    if (vm != null)
                    {
                        await vm.SaveAsync();

                        if (dispose)
                        {
                            vm.Dispose();
                        }
                    }
                }
                catch
                {
                    // if the editor fails to load, there is nothing to save
                    // actual error notification happens through the UI
                }
            }
        }
    }
}
