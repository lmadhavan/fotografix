using Fotografix.Editor;
using Fotografix.IO;
using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Uwp
{
    [TestClass]
    public class ImageEditorTest
    {
        private static readonly Size ImageSize = new Size(10, 10);

        private Image image;
        private Viewport viewport;
        private CommandHandlerCollection handlerCollection;
        private ImageEditor editor;

        [TestInitialize]
        public void Initialize()
        {
            this.image = new Image(ImageSize);
            image.Layers.Add(BitmapLayerFactory.CreateBitmapLayer(id: 1));

            this.viewport = new Viewport();
            image.SetViewport(viewport);

            this.handlerCollection = new CommandHandlerCollection();

            this.editor = new ImageEditor(image, handlerCollection)
            {
                ImageDecoder = new FakeImageCodec()
            };
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
        public void GroupsChangesProducedByCommand()
        {
            handlerCollection.Register(new FakeCommandHandler());

            editor.Dispatch(new FakeCommand(image, numberOfChanges: 3));
            editor.Undo();

            Assert.IsFalse(editor.CanUndo);
        }

        [TestMethod]
        public void SynchronizesViewportWhenImageSizeChanges()
        {
            image.Size = new Size(100, 100);
            Assert.AreEqual(viewport.ImageSize, image.Size);
        }

        private sealed class FakeCommand
        {
            private readonly Image image;
            private readonly int numberOfChanges;

            public FakeCommand(Image image, int numberOfChanges)
            {
                this.image = image;
                this.numberOfChanges = numberOfChanges;
            }

            public void Execute()
            {
                for (int i = 0; i < numberOfChanges; i++)
                {
                    image.Layers.Add(new BitmapLayer());
                }
            }
        }

        private sealed class FakeCommandHandler : ICommandHandler<FakeCommand>
        {
            public void Handle(FakeCommand command)
            {
                command.Execute();
            }
        }
    }
}
