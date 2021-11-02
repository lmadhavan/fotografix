using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;

namespace Fotografix
{
    public sealed class EditorViewModel : NotifyPropertyChangedBase, IDisposable
    {
        private readonly PhotoEditor editor;
        private readonly ICanvasResourceCreatorWithDpi dpiResolver;
        private Size renderSize;

        public EditorViewModel(PhotoEditor editor, ICanvasResourceCreatorWithDpi dpiResolver)
        {
            this.editor = editor;
            editor.PropertyChanged += Editor_PropertyChanged;
            editor.Invalidated += (s, e) => Invalidate();

            this.dpiResolver = dpiResolver;
            UpdateRenderSize();
        }

        public void Dispose()
        {
            editor.Dispose();
        }

        public double RenderWidth => renderSize.Width;
        public double RenderHeight => renderSize.Height;
        
        public IPhotoAdjustment Adjustment => editor.Adjustment;

        public bool ShowOriginal
        {
            get => !editor.AdjustmentEnabled;
            set => editor.AdjustmentEnabled = !value;
        }

        public event EventHandler Invalidated;

        public void SetViewportSize(Size size)
        {
            size.Width = ConvertDipsToPixels(size.Width);
            size.Height = ConvertDipsToPixels(size.Height);
            editor.SetRenderSize(size);
        }

        public void Draw(CanvasDrawingSession ds)
        {
            ds.Units = CanvasUnits.Pixels;
            editor.Draw(ds);
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

        private double ConvertPixelsToDips(double pixels)
        {
            return pixels * 96 / dpiResolver.Dpi;
        }

        private double ConvertDipsToPixels(double dips)
        {
            return dips * dpiResolver.Dpi / 96;
        }

        private void UpdateRenderSize()
        {
            var size = editor.RenderSize;
            size.Width = ConvertPixelsToDips(size.Width);
            size.Height = ConvertPixelsToDips(size.Height);
            this.renderSize = size;
            RaisePropertyChanged(nameof(RenderWidth));
            RaisePropertyChanged(nameof(RenderHeight));
        }

        private void Editor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(editor.AdjustmentEnabled):
                    RaisePropertyChanged(nameof(ShowOriginal));
                    break;

                case nameof(editor.RenderSize):
                    UpdateRenderSize();
                    break;

                default:
                    RaisePropertyChanged(e.PropertyName);
                    break;
            }
        }
    }
}
