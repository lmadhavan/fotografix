using Fotografix.Input;
using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;

namespace Fotografix
{
    public sealed class EditorViewModel : NotifyPropertyChangedBase, IPointerInputHandler, IDisposable
    {
        public const int CropHandleSize = 6;

        private readonly PhotoEditor editor;
        private readonly ScalingHelper scalingHelper;
        private readonly CropTracker cropTracker;
        private IPointerInputHandler activeInputHandler;
        private bool loaded;

        public EditorViewModel(PhotoEditor editor, ICanvasResourceCreatorWithDpi dpiProvider) : this(editor, dpiProvider, new CropTracker())
        {
        }

        public EditorViewModel(PhotoEditor editor, ICanvasResourceCreatorWithDpi dpiProvider, CropTracker cropTracker)
        {
            this.editor = editor;
            editor.Invalidated += (s, e) => Invalidate();

            this.scalingHelper = new ScalingHelper(dpiProvider, editor);
            UpdateRenderSize();

            this.cropTracker = cropTracker;
            cropTracker.RectChanged += (s, e) => Invalidate();

            this.activeInputHandler = new NullPointerInputHandler();
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

        public IPhotoAdjustment Adjustment => editor.Adjustment;

        public bool ShowOriginal
        {
            get => !editor.AdjustmentEnabled;

            set
            {
                editor.AdjustmentEnabled = !value;
                RaisePropertyChanged();
            }
        }

        public void Draw(CanvasDrawingSession ds)
        {
            if (loaded)
            {
                ds.Units = CanvasUnits.Pixels;
                editor.Draw(ds);

                if (cropMode)
                {
                    ds.Units = CanvasUnits.Dips;
                    ds.DrawRectangle(scalingHelper.ImageToScreen(cropTracker.Rect), Colors.Red);
                }
            }
        }

        public event EventHandler Invalidated;

        #region Viewport

        private Size viewportSize;
        private Size renderSize;
        private bool zoomToFit = true;

        public double RenderWidth => renderSize.Width;
        public double RenderHeight => renderSize.Height;

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
                    ScaleToFit();
                }
            }
        }

        public bool IsZoomedToActualPixels => editor.RenderScale == 1;
        public bool IsPreviewAccuracyWarningVisible => editor.RenderScale != 1;

        public bool CanZoomToFit => !zoomToFit;
        public bool CanZoomToActualPixels => zoomToFit && !cropMode;

        public void SetViewportSize(Size size)
        {
            this.viewportSize = scalingHelper.ScreenToViewport(size);

            if (loaded && zoomToFit)
            {
                ScaleToFit();
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
            UpdateRenderSize();
        }

        private void UpdateRenderSize()
        {
            this.renderSize = scalingHelper.ViewportToScreen(editor.RenderSize);
            RaisePropertyChanged(nameof(RenderWidth));
            RaisePropertyChanged(nameof(RenderHeight));
            RaisePropertyChanged(nameof(IsZoomedToActualPixels));
            RaisePropertyChanged(nameof(IsPreviewAccuracyWarningVisible));
        }

        private void ScaleToFit()
        {
            editor.ScaleToFit(viewportSize);
            UpdateRenderSize();
        }

        #endregion

        #region Crop

        private bool cropMode;
        
        public bool CropMode
        {
            get => cropMode;

            set
            {
                if (SetProperty(ref cropMode, value))
                {
                    ZoomToFit();

                    if (cropMode)
                    {
                        BeginCrop();
                    }
                    else
                    {
                        EndCrop();
                    }

                    Invalidate();
                }
            }
        }

        private void BeginCrop()
        {
            cropTracker.MaxBounds = DefaultCropRectangle;
            cropTracker.Rect = Adjustment.Crop ?? DefaultCropRectangle;
            SetCropAdjustment(null);
            cropTracker.HandleTolerance = scalingHelper.ScreenToImage(CropHandleSize);
            this.activeInputHandler = cropTracker;
        }

        private void EndCrop()
        {
            if (cropTracker.Rect == DefaultCropRectangle)
            {
                SetCropAdjustment(null);
            }
            else
            {
                SetCropAdjustment(cropTracker.Rect);
            }

            this.activeInputHandler = new NullPointerInputHandler();
        }

        private void SetCropAdjustment(Rect? rect)
        {
            Adjustment.Crop = rect;
            ScaleToFit();
        }

        private Rect DefaultCropRectangle => new Rect(new Point(), editor.OriginalSize);

        #endregion

        #region Pointer input

        public CoreCursor Cursor => activeInputHandler.Cursor;

        public bool PointerPressed(Point pt)
        {
            return activeInputHandler.PointerPressed(scalingHelper.ScreenToImage(pt));
        }

        public bool PointerMoved(Point pt)
        {
            return activeInputHandler.PointerMoved(scalingHelper.ScreenToImage(pt));
        }

        public bool PointerReleased(Point pt)
        {
            return activeInputHandler.PointerReleased(scalingHelper.ScreenToImage(pt));
        }

        #endregion

        #region File management

        public bool CanRevert => editor.CanRevert;

        public async void Revert()
        {
            this.CropMode = false;

            await editor.RevertAsync();

            ScaleToFit();
            RaisePropertyChanged();
        }

        public void Reset()
        {
            this.CropMode = false;

            editor.Reset();

            ScaleToFit();
            RaisePropertyChanged();
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

        #endregion

        private void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}
