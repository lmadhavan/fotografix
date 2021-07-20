using Fotografix.Editor;
using Fotografix.IO;
using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Drawing;

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
            image.Layers.Add(new Layer());

            this.viewport = new Viewport();
            image.SetViewport(viewport);

            this.document = new Document(image);

            this.editor = new ImageEditor(document);
            editor.PropertyChanged += (s, e) => this.lastPropertyChange = e;
        }

        [TestCleanup]
        public void Cleanup()
        {
            editor.Dispose();
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
