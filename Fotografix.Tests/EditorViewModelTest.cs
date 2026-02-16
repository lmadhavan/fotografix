using Fotografix.Export;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using static Fotografix.CropTracker;

namespace Fotografix
{
    [TestClass]
    public class EditorViewModelTest
    {
        private static readonly Size PhotoSize = new Size(1000, 500);
        private const float DpiScalingFactor = 2;

        private FakePhoto photo;
        private PhotoEditor editor;
        private CropTracker cropTracker;
        private EditorViewModel vm;

        [TestInitialize]
        public async Task Initialize()
        {
            this.photo = new FakePhoto { Size = PhotoSize };
            var dpiProvider = new StubCanvasResourceCreator { ScalingFactor = DpiScalingFactor };
            this.editor = await PhotoEditor.CreateAsync(photo, dpiProvider);

            this.cropTracker = new CropTracker();
            this.vm = new EditorViewModel(editor, dpiProvider, ApplicationData.Current.TemporaryFolder, cropTracker);
        }

        [TestCleanup]
        public void Cleanup()
        {
            vm.Dispose();
        }

        private void SetupRenderScale(float scale)
        {
            SetupRenderScale(scale, PhotoSize);
        }

        private void SetupRenderScale(float scale, Size photoSize)
        {
            // pick a viewport size that results in the desired render scale
            vm.SetViewportSize(new Size(photoSize.Width * scale / DpiScalingFactor, photoSize.Height * scale / DpiScalingFactor));
        }
        
        [TestMethod]
        public void CanZoomToActualPixels()
        {
            vm.ZoomToFit();

            Assert.IsTrue(vm.IsZoomedToFit, nameof(vm.IsZoomedToFit));
            Assert.IsTrue(vm.CanZoomToActualPixels, nameof(vm.CanZoomToActualPixels));
        }

        [TestMethod]
        public void CanZoomToFit()
        {
            vm.ZoomToActualPixels();

            Assert.IsTrue(vm.IsZoomedToActualPixels, nameof(vm.IsZoomedToActualPixels));
            Assert.IsTrue(vm.CanZoomToFit, nameof(vm.CanZoomToFit));
        }

        [TestMethod]
        public void SetsRenderScaleFromViewportSize()
        {
            SetupRenderScale(0.5f);

            Assert.AreEqual(0.5f, editor.RenderScale);
            Assert.AreEqual(250, vm.RenderWidth);
            Assert.AreEqual(125, vm.RenderHeight);
        }

        [TestMethod]
        public void ResetsRenderScaleWhenZoomedToActualPixels()
        {
            SetupRenderScale(0.5f);

            vm.ZoomToActualPixels();

            Assert.AreEqual(1, editor.RenderScale);
        }

        [TestMethod]
        public void RemembersPreviousViewportSizeWhenZoomedToFit()
        {
            SetupRenderScale(0.5f);

            vm.ZoomToActualPixels();
            vm.ZoomToFit();
            
            Assert.AreEqual(0.5f, editor.RenderScale);
        }

        [TestMethod]
        public void EnteringTransformModeLocksZoom()
        {
            vm.ZoomToActualPixels();
            vm.TransformMode = true;

            Assert.IsTrue(vm.IsZoomedToFit, nameof(vm.IsZoomedToFit));
            Assert.IsFalse(vm.CanZoomToActualPixels, nameof(vm.CanZoomToActualPixels));
        }

        [TestMethod]
        public void CropRectangleDefaultsToPhotoSize()
        {
            vm.TransformMode = true;

            Assert.AreEqual(new Rect(new Point(), PhotoSize), cropTracker.Rect);
        }

        [TestMethod]
        public void UsesExistingCropRectangle()
        {
            Size existingCropSize = new Size(250, 250);
            Rect existingCropRect = new Rect(new Point(), existingCropSize);

            editor.Adjustment.Crop = existingCropRect;
            SetupRenderScale(1f, existingCropSize);

            vm.TransformMode = true;

            Assert.AreEqual(existingCropRect, cropTracker.Rect, "tracker rectangle should be initialized to existing crop rectangle");
            Assert.AreEqual(new Rect(new Point(), PhotoSize), cropTracker.MaxBounds, "tracker max bounds should be initialized to photo size");
            Assert.IsNull(editor.Adjustment.Crop, "should display original photo while in crop mode");
            Assert.AreEqual(0.25f, editor.RenderScale, "original photo should be scaled down to fit viewport");
        }

        [TestMethod]
        public void TranslatesPointerInputToImageCoordinates()
        {
            editor.Adjustment.Crop = new Rect(0, 0, 100, 100);
            SetupRenderScale(0.5f);

            vm.TransformMode = true;
            vm.PointerPressed(new Point(0, 0));
            vm.PointerMoved(new Point(10, 10));

            Assert.AreEqual(4 * EditorViewModel.CropHandleSize, cropTracker.HandleTolerance, "crop handle tolerance");
            Assert.AreEqual(RectFromLTRB(40, 40, 100, 100), cropTracker.Rect, "crop rectangle");
        }

        [TestMethod]
        public void ExitingTransformModeUpdatesEditor()
        {
            SetupRenderScale(0.5f);

            vm.TransformMode = true;
            cropTracker.Rect = new Rect(10, 20, 30, 40);
            vm.TransformMode = false;

            Assert.AreEqual(new Rect(10, 20, 30, 40), editor.Adjustment.Crop, "adjustment should contain updated crop rectangle");
            Assert.AreEqual(1f, editor.RenderScale, "cropped photo should fully fit within existing viewport");
        }

        [TestMethod]
        public void DefaultCropResultsInNullCropAdjustment()
        {
            vm.TransformMode = true;
            vm.TransformMode = false;

            Assert.IsNull(editor.Adjustment.Crop);
        }

        [TestMethod]
        public void ExitingTransformModeReenablesZoom()
        {
            vm.TransformMode = true;
            vm.TransformMode = false;

            Assert.IsTrue(vm.CanZoomToActualPixels, nameof(vm.CanZoomToActualPixels));
        }

        [DataTestMethod]
        [DynamicData(nameof(FileManagementActions), DynamicDataSourceType.Method)]
        public void FileManagementActionExitsTransformMode(string name, Action<EditorViewModel> action, bool shouldResetAdjustment)
        {
            Size existingCropSize = new Size(250, 250);
            Rect existingCropRect = new Rect(new Point(), existingCropSize);

            editor.Adjustment.Crop = existingCropRect;
            SetupRenderScale(1f, existingCropSize);

            vm.TransformMode = true;
            action(vm);

            Assert.IsFalse(vm.TransformMode, $"{name} should exit transform mode");

            if (shouldResetAdjustment)
            {
                Assert.IsNull(editor.Adjustment.Crop, $"{name} should reset adjustment");
                Assert.AreEqual(0.25f, editor.RenderScale, $"{name} should recompute render scale");
            }
        }

        private static IEnumerable<object[]> FileManagementActions()
        {
            object[] TestCase(string name, Action<EditorViewModel> action, bool shouldResetAdjustment)
            {
                return new object[] { name, action, shouldResetAdjustment };
            }

            yield return TestCase("reset", vm => vm.Reset(), true);
            yield return TestCase("revert", async vm => await vm.RevertAsync(), true);
            yield return TestCase("save", async vm => await vm.SaveAsync(), false);
            yield return TestCase("export", async vm => await vm.ExportAsync(new NullExportHandler(), "Test"), false);
        }

        [TestMethod]
        public void DefaultsToUnconstrainedAspectRatio()
        {
            vm.TransformMode = true;

            Assert.IsNull(cropTracker.AspectRatio);
        }

        [TestMethod]
        public void FirstAvailableAspectRatioIsOriginalPhotoSize()
        {
            Assert.AreEqual(PhotoSize.Width / PhotoSize.Height, vm.AvailableAspectRatios.First().Value);
        }

        [TestMethod]
        public void ConstrainsCropToSelectedAspectRatio()
        {
            vm.TransformMode = true;
            vm.AspectRatio = new AspectRatio(5, 2);

            Assert.AreEqual(2.5, cropTracker.AspectRatio, "normal aspect ratio");

            vm.FlipAspectRatio = true;

            Assert.AreEqual(0.4, cropTracker.AspectRatio, "flipped aspect ratio");
        }

        [TestMethod]
        public void ResetsTransform()
        {
            vm.TransformMode = true;
            cropTracker.Rect = new Rect(10, 20, 30, 40);
            vm.Rotate();
            vm.FlipPhoto = true;

            vm.ResetTransform();

            Assert.AreEqual(new Rect(new Point(), PhotoSize), cropTracker.Rect);
            Assert.AreEqual(0, editor.Adjustment.Rotation);
            Assert.IsFalse(editor.Adjustment.Flip);
        }

        [TestMethod]
        public void RotatesPhotoIn90DegreeIncrements()
        {
            vm.TransformMode = true;
            
            vm.Rotate();

            Assert.AreEqual(90, editor.Adjustment.Rotation);
        }

        [TestMethod]
        public void RotatingPhotoResetsCropRectangle()
        {
            vm.TransformMode = true;
            
            vm.Rotate();

            Size rotatedPhotoSize = new Size(PhotoSize.Height, PhotoSize.Width);
            Assert.AreEqual(new Rect(new Point(), rotatedPhotoSize), cropTracker.Rect);
        }

        [TestMethod]
        public void RotatingPhotoResetsRenderScale()
        {
            SetupRenderScale(0.5f); // 0.5x @ 1000x500 => 500x250 viewport
            vm.TransformMode = true;
            
            vm.Rotate();

            Assert.AreEqual(0.25f, editor.RenderScale); // 500x250 viewport => 0.25x @ 500x1000
        }

        [TestMethod]
        public void FlippingPhotoFlipsCropRectangle()
        {
            vm.TransformMode = true;
            cropTracker.Rect = new Rect(10, 20, 30, 40);

            vm.FlipPhoto = true;

            Assert.IsTrue(editor.Adjustment.Flip);
            Assert.AreEqual(new Rect(PhotoSize.Width - 40, 20, 30, 40), cropTracker.Rect);
        }

        [TestMethod]
        public void OriginalAspectRatioRespectsPhotoOrientation()
        {
            vm.TransformMode = true;

            vm.Rotate();

            Assert.AreEqual(PhotoSize.Height / PhotoSize.Width, vm.AvailableAspectRatios.First().Value);
        }

        [TestMethod]
        public void IgnoresDrawCallsAfterDispose()
        {
            Assert.IsTrue(vm.IsLoaded);

            vm.Dispose();
            vm.Draw(ds: null /* don't care */);

            Assert.IsFalse(vm.IsLoaded);
        }

        [TestMethod]
        public void IgnoresViewportSizeAfterDispose()
        {
            Assert.IsTrue(vm.IsLoaded);

            vm.Dispose();
            vm.SetViewportSize(new Size(10, 10));

            Assert.IsFalse(vm.IsLoaded);
        }
    }
}
