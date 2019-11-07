using Fotografix.Adjustments;
using Fotografix.Composition;
using Fotografix.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fotografix.Tests.UI
{
    [TestClass]
    public class ImageEditorTest
    {
        private Image image;
        private Layer layer;
        private ImageEditor editor;

        [TestInitialize]
        public void Initialize()
        {
            this.image = new Image(CanvasDevice.GetSharedDevice(), 1, 1);
            this.layer = new AdjustmentLayer(new BlackAndWhiteAdjustment());
            this.editor = new ImageEditor(image);
        }

        [TestCleanup]
        public void Cleanup()
        {
            editor.Dispose();
            layer.Dispose();
        }

        [TestMethod]
        public void AddsLayer()
        {
            editor.AddLayer(layer);

            Assert.AreEqual(2, editor.Layers.Count);
            Assert.AreEqual(layer, editor.SelectedLayer);
        }

        [TestMethod]
        public void DeletesLayer()
        {
            Assert.IsFalse(editor.CanDeleteLayer, "Should not be able to delete bottom layer");

            editor.AddLayer(layer);

            Assert.IsTrue(editor.CanDeleteLayer, "Should be able to delete newly added layer");

            editor.DeleteLayer();

            Assert.AreEqual(1, editor.Layers.Count);
            Assert.AreNotEqual(layer, editor.SelectedLayer);

            Assert.IsFalse(editor.CanDeleteLayer, "Should not be able to delete after deleting added layer");
        }

        [TestMethod]
        public void ExposesLayersInReverseOrder()
        {
            Assert.AreEqual(image.Layers[0], editor.Layers[0]);
         
            editor.AddLayer(layer);

            Assert.AreEqual(image.Layers[1], editor.Layers[0]);
            Assert.AreEqual(image.Layers[0], editor.Layers[1]);
        }

        [TestMethod]
        public void BlendModeNotEnabledOnBottomLayer()
        {
            editor.AddLayer(layer);

            editor.SelectedLayer = editor.Layers.Last();
            Assert.IsFalse(editor.IsBlendModeEnabled);

            editor.SelectedLayer = editor.Layers.First();
            Assert.IsTrue(editor.IsBlendModeEnabled);
        }
    }
}
