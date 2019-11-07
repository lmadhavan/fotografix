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
        public void ActiveLayerFollowsUpdatesToLayerList()
        {
            image.Layers.Add(layer);

            Assert.AreEqual(layer, editor.ActiveLayer);

            Layer anotherLayer = new AdjustmentLayer(new HueSaturationAdjustment());
            image.Layers[1] = anotherLayer;

            Assert.AreEqual(anotherLayer, editor.ActiveLayer);

            editor.Layers.Remove(anotherLayer);

            Assert.AreEqual(image.Layers[0], editor.ActiveLayer);
        }

        [TestMethod]
        public void DeletesActiveLayer()
        {
            Assert.IsFalse(editor.CanDeleteActiveLayer, "Should not be able to delete bottom layer");

            editor.AddLayer(layer);

            Assert.IsTrue(editor.CanDeleteActiveLayer, "Should be able to delete newly added layer");

            editor.DeleteActiveLayer();

            Assert.AreEqual(image.Layers[0], editor.ActiveLayer);
            Assert.IsFalse(editor.CanDeleteActiveLayer, "Should not be able to delete bottom layer");
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

            editor.ActiveLayer = editor.Layers.Last();
            Assert.IsFalse(editor.IsBlendModeEnabled);

            editor.ActiveLayer = editor.Layers.First();
            Assert.IsTrue(editor.IsBlendModeEnabled);
        }
    }
}
