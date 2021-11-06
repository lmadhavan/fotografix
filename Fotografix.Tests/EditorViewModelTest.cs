using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Foundation;

namespace Fotografix
{
    [TestClass]
    public class EditorViewModelTest
    {
        private FakePhotoEditor editor;
        private StubCanvasResourceCreator dpiResolver;
        private EditorViewModel vm;

        [TestInitialize]
        public void Initialize()
        {
            this.editor = new FakePhotoEditor();
            this.dpiResolver = new StubCanvasResourceCreator();
            this.vm = new EditorViewModel(editor, dpiResolver);
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
        public void SetsRenderSizeToViewportSize()
        {
            dpiResolver.Dpi = 96 * 2; // 200% scaling
            Size sizeInDips = new Size(200, 100);
            Size expectedSizeInPixels = new Size(400, 200);

            vm.SetViewportSize(sizeInDips);
            
            Assert.AreEqual(expectedSizeInPixels, editor.RenderSize);
        }

        [TestMethod]
        public void ResetsRenderScaleWhenZoomedToActualPixels()
        {
            editor.RenderScale = 0.5f;

            vm.ZoomToActualPixels();

            Assert.AreEqual(1, editor.RenderScale);
            Assert.AreEqual(default, editor.RenderSize);
        }

        public void RestoresPreviousViewportSizeWhenZoomedToFit()
        {
            dpiResolver.Dpi = 96 * 2; // 200% scaling
            Size sizeInDips = new Size(200, 100);
            Size expectedSizeInPixels = new Size(400, 200);

            vm.SetViewportSize(sizeInDips);
            vm.ZoomToActualPixels();
            vm.ZoomToFit();
            
            Assert.AreEqual(expectedSizeInPixels, editor.RenderSize);
        }

        [TestMethod]
        public void ConvertsEditorRenderSizeToDips()
        {
            dpiResolver.Dpi = 96 * 2; // 200% scaling
            Size sizeInPixels = new Size(200, 100);
            Size expectedSizeInDips = new Size(100, 50);

            editor.RenderSize = sizeInPixels;
            
            Assert.AreEqual(expectedSizeInDips.Width, vm.RenderWidth, nameof(vm.RenderWidth));
            Assert.AreEqual(expectedSizeInDips.Height, vm.RenderHeight, nameof(vm.RenderHeight));
        }
    }
}
