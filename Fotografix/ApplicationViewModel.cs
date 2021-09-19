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
        private NotifyTaskCompletion<PhotoEditor> editor;

        public Task DisposeAsync()
        {
            return DisposeEditorAsync();
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

        private async Task<IList<PhotoViewModel>> LoadPhotosAsync(StorageFolder folder)
        {
            var photos = await PhotoFolder.GetPhotosAsync(folder);
            return photos.Select(p => new PhotoViewModel(p)).ToList();
        }

        private async Task<PhotoEditor> LoadEditorAsync()
        {
            await DisposeEditorAsync();

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

        private async Task DisposeEditorAsync()
        {
            if (editor != null)
            {
                (await editor.Task)?.Dispose();
            }
        }
    }
}
