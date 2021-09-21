using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Fotografix
{
    public sealed class ApplicationViewModel : NotifyPropertyChangedBase
    {
        private readonly ISidecarStrategy sidecarStrategy;

        private NotifyTaskCompletion<IList<PhotoViewModel>> photos;
        private PhotoViewModel selectedPhoto;
        private NotifyTaskCompletion<PhotoEditor> editor;

        public ApplicationViewModel(ISidecarStrategy sidecarStrategy)
        {
            this.sidecarStrategy = sidecarStrategy;
        }

        public Task DisposeAsync()
        {
            return SaveAsync(dispose: true);
        }

        public async void PickFolder()
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                OpenFolder(folder);
            }
        }

        public void OpenFolder(StorageFolder folder)
        {
            this.Photos = new NotifyTaskCompletion<IList<PhotoViewModel>>(LoadPhotosAsync(folder));
        }

        public NotifyTaskCompletion<IList<PhotoViewModel>> Photos
        {
            get => photos;
            private set => SetProperty(ref photos, value);
        }

        public PhotoViewModel SelectedPhoto
        {
            get => selectedPhoto;

            set
            {
                if (SetProperty(ref selectedPhoto, value))
                {
                    this.Editor = new NotifyTaskCompletion<PhotoEditor>(LoadEditorAsync());
                    Editor.Task.ContinueWith(t => InvalidateEditor());
                }
            }
        }

        public NotifyTaskCompletion<PhotoEditor> Editor
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

        private async Task<PhotoEditor> LoadEditorAsync()
        {
            await SaveAsync(dispose: true);

            if (selectedPhoto == null)
            {
                return null;
            }

            var editor = await selectedPhoto.CreateEditorAsync();
            editor.Invalidated += (s, e) => InvalidateEditor();
            return editor;
        }

        private void InvalidateEditor()
        {
            EditorInvalidated?.Invoke(this, EventArgs.Empty);
        }

        private async Task SaveAsync(bool dispose)
        {
            if (editor != null)
            {
                var photoEditor = await editor.Task;
                if (photoEditor != null)
                {
                    await photoEditor.SaveAsync();

                    if (dispose)
                    {
                        photoEditor.Dispose();
                    }
                }
            }
        }
    }
}
