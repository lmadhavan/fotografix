﻿using Fotografix.Editor;
using Fotografix.Editor.Commands;
using Fotografix.IO;
using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Uwp
{
    [TestClass]
    public class ImageEditorTest
    {
        private const string Filename = "file.tst";
        private static readonly Size ImageSize = new Size(10, 10);

        private Image image;
        private Viewport viewport;
        private Document document;
        private ImageEditor editor;
        private PropertyChangedEventArgs lastPropertyChange;

        [TestInitialize]
        public void Initialize()
        {
            this.image = new Image(ImageSize);
            image.Layers.Add(ImageEditor.CreateLayer(id: 1));

            this.viewport = new Viewport();
            image.SetViewport(viewport);

            this.document = new Document(image);

            this.editor = new ImageEditor(document, new CommandHandlerCollection())
            {
                ImageDecoder = new FakeImageCodec()
            };
            editor.PropertyChanged += (s, e) => this.lastPropertyChange = e;
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

            var files = new IFile[] {
                new InMemoryFile(filename1),
                new InMemoryFile(filename2)
            };

            await editor.ImportLayersAsync(files);

            Assert.AreEqual(3, editor.Layers.Count);
            Assert.AreEqual(filename2, editor.Layers[0].Name);
            Assert.AreEqual(filename1, editor.Layers[1].Name);
        }

        [TestMethod]
        public void SynchronizesViewportWhenImageSizeChanges()
        {
            image.Size = new Size(100, 100);
            Assert.AreEqual(viewport.ImageSize, image.Size);
        }

        [TestMethod]
        public void DisplaysFilenameInTitle()
        {
            document.File = new InMemoryFile(Filename);

            AssertPropertyChanged(nameof(ImageEditor.Title));
            Assert.AreEqual(Filename, editor.Title);
        }

        [TestMethod]
        public void DisplaysDirtyIndicatorInTitle()
        {
            document.File = new InMemoryFile(Filename);
            document.IsDirty = true;

            AssertPropertyChanged(nameof(ImageEditor.Title));
            Assert.AreEqual("* " + Filename, editor.Title);
        }

        [TestMethod]
        public void DisplaysDefaultTitleWhenNoFilenameIsPresent()
        {
            Assert.AreEqual("Untitled", editor.Title);
        }

        private void AssertPropertyChanged(string propertyName)
        {
            Assert.IsNotNull(lastPropertyChange, "Property change event");
            Assert.AreEqual(propertyName, lastPropertyChange.PropertyName, "Property name");
        }
    }
}
