using Fotografix.Export;
using Fotografix.Input;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace Fotografix
{
    public sealed class EditorViewModel : NotifyPropertyChangedBase, IPointerInputHandler, IDisposable
    {
        public const int CropHandleSize = 6;

        private readonly PhotoEditor editor;
        private readonly ICanvasResourceCreatorWithDpi resourceCreator;
        private readonly IExportHandler exportHandler;
        private readonly ScalingHelper scalingHelper;
        private readonly CropTracker cropTracker;
        private readonly CropOverlay cropOverlay;
        private IPointerInputHandler activeInputHandler;
        private bool loaded;

        public EditorViewModel(PhotoEditor editor, ICanvasResourceCreatorWithDpi resourceCreator, IExportHandler exportHandler) : this(editor, resourceCreator, exportHandler, new CropTracker())
        {
        }

        public EditorViewModel(PhotoEditor editor, ICanvasResourceCreatorWithDpi resourceCreator, IExportHandler exportHandler, CropTracker cropTracker)
        {
            this.editor = editor;
            editor.Invalidated += (s, e) => Invalidate();

            this.resourceCreator = resourceCreator;
            this.exportHandler = exportHandler;
            this.scalingHelper = new ScalingHelper(resourceCreator, editor);
            this.cropTracker = cropTracker;
            cropTracker.RectChanged += (s, e) => Invalidate();
            this.cropOverlay = new CropOverlay(resourceCreator, scalingHelper) { HandleSize = CropHandleSize * 3 };

            ResetAspectRatios();
            UpdateRenderSize();
            this.activeInputHandler = new NullPointerInputHandler();

            this.RevertCommand = new DelegateCommand(() => CanRevert, RevertAsync);
            this.ResetCommand = new DelegateCommand(Reset);
            this.ExportCommand = new DelegateCommand(ExportAsync);
            this.QuickExportCommand = new DelegateCommand(QuickExportAsync);

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

                if (transformMode)
                {
                    ds.Units = CanvasUnits.Dips;
                    cropOverlay.Draw(ds, cropTracker);
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
        public bool CanZoomToActualPixels => zoomToFit && !transformMode;

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
            cropTracker.HandleTolerance = scalingHelper.ScreenToImage(CropHandleSize);

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

        #region Transform

        private bool transformMode;
        private AspectRatio aspectRatio;
        private bool flipAspectRatio;

        public bool TransformMode
        {
            get => transformMode;

            set
            {
                if (SetProperty(ref transformMode, value))
                {
                    ZoomToFit();

                    if (transformMode)
                    {
                        BeginTransform();
                    }
                    else
                    {
                        EndTransform();
                    }

                    RaisePropertyChanged(nameof(CanZoomToActualPixels));
                    Invalidate();
                }
            }
        }

        public List<AspectRatio> AvailableAspectRatios { get; set; }
        private AspectRatio DefaultAspectRatio => AspectRatio.Unconstrained;

        public AspectRatio AspectRatio
        {
            get => aspectRatio;

            set
            {
                if (SetProperty(ref aspectRatio, value ?? DefaultAspectRatio))
                {
                    this.FlipAspectRatio = false;
                    UpdateAspectRatio();
                }
            }
        }

        public bool FlipAspectRatio
        {
            get => flipAspectRatio;

            set
            {
                if (SetProperty(ref flipAspectRatio, value))
                {
                    UpdateAspectRatio();
                }
            }
        }

        public bool FlipPhoto
        {
            get => Adjustment.Flip;

            set
            {
                if (Adjustment.Flip != value)
                {
                    Adjustment.Flip = value;
                    cropTracker.Flip();
                }
            }
        }

        public void ResetTransform()
        {
            Adjustment.Rotation = 0;
            Adjustment.Straighten = 0;
            this.FlipPhoto = false;

            ResetCrop();
        }

        public void Rotate()
        {
            Adjustment.Rotation += 90;
            ResetCrop();
        }

        private void ResetAspectRatios()
        {
            Size orientedSize = editor.OrientedSize;
            AvailableAspectRatios = new List<AspectRatio>();
            AvailableAspectRatios.Add(new AspectRatio(orientedSize.Width, orientedSize.Height, "Original"));
            AvailableAspectRatios.Add(AspectRatio.Unconstrained);
            AvailableAspectRatios.AddRange(AspectRatio.StandardRatios);
            AspectRatio = DefaultAspectRatio;

            RaisePropertyChanged(nameof(AvailableAspectRatios));
            RaisePropertyChanged(nameof(AspectRatio));
        }

        private void ResetCrop()
        {
            ResetAspectRatios();
            cropTracker.Rect = cropTracker.MaxBounds = DefaultCropRectangle;
            ScaleToFit();
        }

        private void UpdateAspectRatio()
        {
            cropTracker.AspectRatio = flipAspectRatio ? aspectRatio.InverseValue : aspectRatio.Value;
        }

        private void BeginTransform()
        {
            var maxBounds = DefaultCropRectangle;
            cropTracker.MaxBounds = maxBounds;
            cropTracker.Rect = Adjustment.Crop ?? maxBounds;
            SetCropAdjustment(null);
            this.activeInputHandler = cropTracker;
        }

        private void EndTransform()
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

        private Rect DefaultCropRectangle => new Rect(new Point(), editor.OrientedSize);

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

        public ICommand RevertCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand QuickExportCommand { get; }
        
        public bool CanRevert => editor.CanRevert;

        public async Task RevertAsync()
        {
            this.TransformMode = false;

            await editor.RevertAsync();

            ScaleToFit();
            RaiseAllPropertiesChanged();
        }

        public void Reset()
        {
            this.TransformMode = false;

            editor.Reset();

            ScaleToFit();
            RaiseAllPropertiesChanged();
        }

        public Task SaveAsync()
        {
            this.TransformMode = false;
            return editor.SaveAsync();
        }

        public Task ExportAsync()
        {
            return ExportAsync(showDialog: true, "Export");
        }

        public Task QuickExportAsync()
        {
            return ExportAsync(showDialog: false, "QuickExport");
        }

        private async Task ExportAsync(bool showDialog, string eventName)
        {
            this.TransformMode = false;
            await exportHandler.ExportAsync(new[] { editor }, showDialog);
            Logger.LogEvent(eventName);
        }

        #endregion

        #region Histogram

        private CanvasImageSource histogramSource;

        public ImageSource HistogramSource
        {
            get
            {
                if (histogramSource == null)
                {
                    this.histogramSource = new CanvasImageSource(resourceCreator, Histogram.RenderSize);
                    editor.Invalidated += (s, e) => UpdateHistogram();
                    UpdateHistogram();
                }

                return histogramSource;
            }
        }

        private void UpdateHistogram()
        {
            var histogram = editor.ComputeHistogram();
            using (var ds = histogramSource.CreateDrawingSession(Color.FromArgb(192, 48, 48, 48)))
            {
                histogram.Draw(ds);
            }
        }

        #endregion

        private void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}
