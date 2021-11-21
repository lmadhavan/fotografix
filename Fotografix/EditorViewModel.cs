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
        private readonly IPhotoEditor editor;
        private readonly ICanvasResourceCreatorWithDpi dpiResolver;
        private bool loaded;
        private Size renderSize;

        public EditorViewModel(IPhotoEditor editor, ICanvasResourceCreatorWithDpi dpiResolver)
        {
            this.editor = editor;
            editor.PropertyChanged += Editor_PropertyChanged;
            editor.Invalidated += (s, e) => Invalidate();

            this.dpiResolver = dpiResolver;
            UpdateRenderSize();

            this.loaded = true;
        }

        public void Dispose()
        {
            this.IsLoaded = false;
            editor.Dispose();
        }

        public bool IsLoaded
        {
            get => loaded;
            private set => SetProperty(ref loaded, value);
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

        #region Viewport controls

        private Size viewportSize;
        private bool zoomToFit = true;

        public bool IsZoomedToFit
        {
            get => zoomToFit;

            private set
            {
                if (SetProperty(ref zoomToFit, value))
                {
                    RaisePropertyChanged(nameof(CanZoomToFit));
                    RaisePropertyChanged(nameof(CanZoomToActualPixels));
                }
                
                if (zoomToFit)
                {
                    editor.SetRenderSize(viewportSize);
                }
            }
        }

        public bool IsZoomedToActualPixels => editor.RenderScale == 1;
        public bool IsPreviewAccuracyWarningVisible => editor.RenderScale != 1;

        public bool CanZoomToFit => !zoomToFit;
        public bool CanZoomToActualPixels => zoomToFit;

        public void SetViewportSize(Size size)
        {
            this.viewportSize = size;
            viewportSize.Width = ConvertDipsToPixels(size.Width);
            viewportSize.Height = ConvertDipsToPixels(size.Height);

            if (loaded && zoomToFit)
            {
                editor.SetRenderSize(viewportSize);
            }
        }

        public void ZoomToFit()
        {
            this.IsZoomedToFit = true;
        }

        public void ZoomToActualPixels()
        {
            editor.RenderScale = 1;
            this.IsZoomedToFit = false;
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

        #endregion

        public void Draw(CanvasDrawingSession ds)
        {
            if (loaded)
            {
                ds.Units = CanvasUnits.Pixels;
                editor.Draw(ds);
            }
        }

        public bool CanRevert => editor.CanRevert;

        public async void Revert()
        {
            await editor.RevertAsync();
        }

        public void Reset()
        {
            editor.Reset();
        }

        public Task SaveAsync()
        {
            return editor.SaveAsync();
        }

        public async void Export()
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
                case nameof(editor.AdjustmentEnabled):
                    RaisePropertyChanged(nameof(ShowOriginal));
                    break;

                case nameof(editor.RenderSize):
                    UpdateRenderSize();
                    RaisePropertyChanged(nameof(IsZoomedToActualPixels));
                    RaisePropertyChanged(nameof(IsPreviewAccuracyWarningVisible));
                    break;

                default:
                    RaisePropertyChanged(e.PropertyName);
                    break;
            }
        }
    }
}
