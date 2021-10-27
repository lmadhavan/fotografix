using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace Fotografix
{
    public sealed class EditorViewModel : NotifyPropertyChangedBase, IDisposable
    {
        private readonly PhotoEditor editor;
        private bool showOriginal;

        public EditorViewModel(PhotoEditor editor)
        {
            this.editor = editor;
            editor.PropertyChanged += Editor_PropertyChanged;
            editor.Invalidated += (s, e) => Invalidate();
        }

        public void Dispose()
        {
            editor.Dispose();
        }

        public double Width => editor.Size.Width;
        public double Height => editor.Size.Height;
        public PhotoAdjustment Adjustment => editor.Adjustment;

        public bool ShowOriginal
        {
            get => showOriginal;

            set
            {
                if (SetProperty(ref showOriginal, value))
                {
                    Invalidate();
                }
            }
        }

        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds)
        {
            editor.Draw(ds, showOriginal);
        }

        public void ResetAdjustment()
        {
            editor.ResetAdjustment();
        }

        public Task SaveAsync()
        {
            return editor.SaveAsync();
        }

        public async Task ExportAsync()
        {
            var folder = ApplicationData.Current.TemporaryFolder;
            var file = await editor.ExportAsync(folder);

            var launcherOptions = new FolderLauncherOptions();
            launcherOptions.ItemsToSelect.Add(file);
            await Launcher.LaunchFolderAsync(folder, launcherOptions);
        }

        private void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        private void Editor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(editor.Size):
                    RaisePropertyChanged(nameof(Width));
                    RaisePropertyChanged(nameof(Height));
                    break;

                default:
                    RaisePropertyChanged(e.PropertyName);
                    break;
            }
        }
    }
}
