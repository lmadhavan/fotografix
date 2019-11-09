using Fotografix.Adjustments;
using Fotografix.Composition;
using Fotografix.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Tests.UI
{
    [TestClass]
    public class ImageEditorTest : ImageTestBase
    {
        private ImageEditor editor;
        private Layer background;
        private Layer foreground;

        [TestInitialize]
        public void Initialize()
        {
            this.editor = new ImageEditor(CanvasDevice.GetSharedDevice(), 1, 1);
            this.background = editor.Layers[0];
            this.foreground = CreateLayer();
        }

        [TestCleanup]
        public void Cleanup()
        {
            editor.Dispose();
            foreground.Dispose();
        }

        [TestMethod]
        public void AddsLayerToTopOfLayerStack()
        {
            editor.AddLayer(foreground);

            Assert.AreEqual(foreground, editor.Layers[0]);
            Assert.AreEqual(background, editor.Layers[1]);
        }

        [TestMethod]
        public void ActiveLayerFollowsUpdatesToLayerStack()
        {
            editor.AddLayer(foreground);

            Assert.AreEqual(foreground, editor.ActiveLayer);

            using (Layer anotherLayer = CreateLayer())
            {
                editor.Layers[0] = anotherLayer;

                Assert.AreEqual(anotherLayer, editor.ActiveLayer);

                editor.Layers.Remove(anotherLayer);
            }

            Assert.AreEqual(background, editor.ActiveLayer);
        }

        [TestMethod]
        public void DeletesActiveLayer()
        {
            Assert.IsFalse(editor.CanDeleteActiveLayer, "Should not be able to delete bottom layer");

            editor.AddLayer(foreground);

            Assert.IsTrue(editor.CanDeleteActiveLayer, "Should be able to delete newly added layer");

            editor.DeleteActiveLayer();

            Assert.AreEqual(background, editor.ActiveLayer);
            Assert.IsFalse(editor.CanDeleteActiveLayer, "Should not be able to delete bottom layer");
        }

        [TestMethod]
        public void BlendModeNotEnabledOnBottomLayer()
        {
            editor.AddLayer(foreground);

            editor.ActiveLayer = editor.Layers.Last();
            Assert.IsFalse(editor.IsBlendModeEnabled);

            editor.ActiveLayer = editor.Layers.First();
            Assert.IsTrue(editor.IsBlendModeEnabled);
        }

        [TestMethod]
        public async Task ImportsFilesAsLayers()
        {
            const string filename1 = "flowers_bw.png";
            const string filename2 = "flowers_hsl.png";

            var files = new StorageFile[] {
                await GetFileAsync(filename1),
                await GetFileAsync(filename2)
            };

            await editor.ImportAsync(files);

            Assert.AreEqual(3, editor.Layers.Count);
            Assert.AreEqual(filename2, editor.Layers[0].Name);
            Assert.AreEqual(filename1, editor.Layers[1].Name);
        }

        private static Layer CreateLayer()
        {
            return new AdjustmentLayer(new BlackAndWhiteAdjustment());
        }
    }
}
