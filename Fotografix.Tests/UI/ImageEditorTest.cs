﻿using Fotografix.Editor.Testing;
using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Tests.UI
{
    [TestClass]
    public class ImageEditorTest
    {
        private static readonly Size ImageSize = new Size(10, 10);

        private ImageEditor editor;

        [TestInitialize]
        public void Initialize()
        {
            this.editor = ImageEditor.Create(ImageSize, new FakeViewport());
        }

        [TestCleanup]
        public void Cleanup()
        {
            editor.Dispose();
        }

        [TestMethod]
        public void AddsNewLayer()
        {
            editor.AddLayer();

            Assert.AreEqual(2, editor.Layers.Count);
            Assert.AreEqual(editor.Layers[0], editor.ActiveLayer);

            Assert.AreEqual("Layer 2", editor.Layers[0].Name);
            Assert.AreEqual("Layer 1", editor.Layers[1].Name);
        }

        [TestMethod]
        public void DeletesActiveLayer()
        {
            Assert.IsFalse(editor.CanDeleteActiveLayer, "Should not be able to delete bottom layer");

            editor.AddLayer();

            Assert.IsTrue(editor.CanDeleteActiveLayer, "Should be able to delete newly added layer");

            editor.DeleteActiveLayer();

            Assert.AreEqual(1, editor.Layers.Count);
            Assert.AreEqual(editor.Layers[0], editor.ActiveLayer);

            Assert.IsFalse(editor.CanDeleteActiveLayer, "Should not be able to delete bottom layer");
        }

        [TestMethod]
        public async Task ImportsFilesAsLayers()
        {
            const string filename1 = "flowers_bw.png";
            const string filename2 = "flowers_hsl.png";

            var files = new StorageFile[] {
                await TestImages.GetFileAsync(filename1),
                await TestImages.GetFileAsync(filename2)
            };

            await editor.ImportLayersAsync(files);

            Assert.AreEqual(3, editor.Layers.Count);
            Assert.AreEqual(filename2, editor.Layers[0].Name);
            Assert.AreEqual(filename1, editor.Layers[1].Name);
        }

        [TestMethod]
        public void ResizesImage()
        {
            var parameters = editor.CreateResizeImageParameters();
            
            Assert.AreEqual(ImageSize, parameters.Size, "Resize parameters should be initialized to current image size");

            Size newImageSize = ImageSize * 2;
            parameters.Width = newImageSize.Width;
            parameters.Height = newImageSize.Height;

            editor.ResizeImage(parameters);

            Assert.AreEqual(newImageSize, editor.Size, "Image should have new size after resizing");
        }
    }
}
