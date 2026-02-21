using Fotografix.Export;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace Fotografix
{
    public sealed class ApplicationViewModel : NotifyPropertyChangedBase
    {
        private readonly ISidecarStrategy sidecarStrategy;

        private StorageFolder folder;
        private NotifyTaskCompletion<IList<PhotoViewModel>> photos;
        private PhotoViewModel activePhoto;
        private IList<PhotoViewModel> selectedPhotos;

        public ApplicationViewModel(ISidecarStrategy sidecarStrategy)
        {
            this.sidecarStrategy = sidecarStrategy;
            this.selectedPhotos = new List<PhotoViewModel>();
            this.BatchExportCommand = new DelegateCommand(BatchExportAsync);
        }

        public Task DisposeAsync()
        {
            CancelEditorLoad();
            return SaveAsync(dispose: true);
        }

        public ICanvasResourceCreatorWithDpi CanvasResourceCreator { get; set; }

        public StorageFolder Folder
        {
            get => folder;

            set
            {
                if (SetProperty(ref folder, value))
                {
                    this.Photos = new NotifyTaskCompletion<IList<PhotoViewModel>>(LoadPhotosAsync(folder));
                    Logger.LogEvent("OpenFolder");
                }
            }
        }

        public NotifyTaskCompletion<IList<PhotoViewModel>> Photos
        {
            get => photos;

            private set
            {
                if (SetProperty(ref photos, value))
                {
                    this.SelectedPhotos = new List<PhotoViewModel>();
                }
            }
        }

        public PhotoViewModel ActivePhoto
        {
            get => activePhoto;

            private set
            {
                if (SetProperty(ref activePhoto, value))
                {
                    CancelEditorLoad();
                    this.editorLoadCts = new CancellationTokenSource();
                    this.Editor = new NotifyTaskCompletion<EditorViewModel>(LoadEditorAsync(editorLoadCts.Token));
                }
            }
        }

        public IList<PhotoViewModel> SelectedPhotos
        {
            get => selectedPhotos;

            set
            {
                if (SetProperty(ref selectedPhotos, value))
                {
                    this.ActivePhoto = selectedPhotos.Count == 1 ? selectedPhotos[0] : null;
                    RaisePropertyChanged(nameof(CanBatchExport));
                }
            }
        }

        #region Editor

        private NotifyTaskCompletion<EditorViewModel> editor;
        private CancellationTokenSource editorLoadCts;
        private string saveError;

        public NotifyTaskCompletion<EditorViewModel> Editor
        {
            get => editor;
            private set => SetProperty(ref editor, value);
        }

        public event EventHandler<EditorViewModel> EditorLoaded;
        public event EventHandler<EditorViewModel> EditorSaving;

        public bool HasSaveError => SaveError != null;

        public string SaveError
        {
            get => saveError;

            private set
            {
                if (SetProperty(ref saveError, value))
                {
                    RaisePropertyChanged(nameof(HasSaveError));
                }
            }
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

            var result = photos.Select(p => new PhotoViewModel(p)).ToList();
            this.ActivePhoto = result.FirstOrDefault();
            return result;
        }

        private async Task<EditorViewModel> LoadEditorAsync(CancellationToken token)
        {
            await SaveAsync(dispose: true);

            token.ThrowIfCancellationRequested();

            if (activePhoto == null)
            {
                return null;
            }

            var editor = await activePhoto.CreateEditorAsync(CanvasResourceCreator);
            var vm = new EditorViewModel(editor, CanvasResourceCreator, folder);
            EditorLoaded?.Invoke(this, vm);
            return vm;
        }

        private void CancelEditorLoad()
        {
            if (editorLoadCts != null)
            {
                editorLoadCts.Cancel();
                editorLoadCts.Dispose();
            }
        }

        private async Task SaveAsync(bool dispose)
        {
            if (editor == null)
            {
                return;
            }

            EditorViewModel vm = null;

            try
            {
                vm = await editor.Task;
            }
            catch
            {
                // if the editor fails to load, there is nothing to save
                // actual error notification happens through the UI
            }

            if (vm != null)
            {
                try
                {
                    this.SaveError = null;
                    EditorSaving?.Invoke(this, vm);
                    await vm.SaveAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error saving changes: {e.Message}");
                    Logger.LogEvent("SaveError");
                    this.SaveError = e.Message;
                }

                if (dispose)
                {
                    vm.Dispose();
                }
            }
        }

        #endregion

        #region Batch export

        private ExportProgressViewModel batchExportProgress;

        public bool CanBatchExport => selectedPhotos.Count > 1;
        public ICommand BatchExportCommand { get; }

        public ExportProgressViewModel BatchExportProgress
        {
            get => batchExportProgress;
            private set => SetProperty(ref batchExportProgress, value);
        }

        private async Task BatchExportAsync()
        {
            var cts = new CancellationTokenSource();

            try
            {
                this.BatchExportProgress = new ExportProgressViewModel(cts);

                var items = selectedPhotos.Select(p => new ExportWrapper(p, CanvasResourceCreator)).ToList();
                var exportHandler = new FileExportHandler(folder)
                {
                    ShowDialog = true,
                    CancellationToken = cts.Token,
                    Progress = batchExportProgress
                };
                await exportHandler.ExportAsync(items);
                Logger.LogEvent("BatchExport");
            }
            finally
            {
                cts.Dispose();
            }
        }

        private sealed class ExportWrapper : IExportable
        {
            private readonly PhotoViewModel photo;
            private readonly ICanvasResourceCreatorWithDpi canvasResourceCreator;

            public ExportWrapper(PhotoViewModel photo, ICanvasResourceCreatorWithDpi canvasResourceCreator)
            {
                this.photo = photo;
                this.canvasResourceCreator = canvasResourceCreator;
            }

            public string Name => photo.Name;

            public async Task<StorageFile> ExportAsync(ExportOptions options)
            {
                using (var editor = await photo.CreateEditorAsync(canvasResourceCreator))
                {
                    return await editor.ExportAsync(options);
                }
            }
        }

        #endregion
    }
}
