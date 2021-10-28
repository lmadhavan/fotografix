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

        public void OpenFolder(StorageFolder folder)
        {
            this.FolderName = folder.DisplayName;
            this.Photos = new NotifyTaskCompletion<IList<PhotoViewModel>>(LoadPhotosAsync(folder));
        }

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
                    Editor.Task.ContinueWith(t => InvalidateEditor());
                }
            }
        }

        public NotifyTaskCompletion<EditorViewModel> Editor
        {
            get => editor;
            private set => SetProperty(ref editor, value);
        }

        public event EventHandler EditorInvalidated;

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

            var editor = await selectedPhoto.CreateEditorAsync();
            var vm = new EditorViewModel(editor);
            vm.Invalidated += (s, e) => InvalidateEditor();
            return vm;
        }

        private void InvalidateEditor()
        {
            EditorInvalidated?.Invoke(this, EventArgs.Empty);
        }

        private async Task SaveAsync(bool dispose)
        {
            if (editor != null)
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
        }
    }
}
