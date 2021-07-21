using Fotografix.Editor.FileManagement;
using Fotografix.IO;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Fotografix.Editor.Layers
{
    [TestFixture]
    public class ImportLayerCommandTest
    {
        private FakeImageCodec imageCodec;
        private FakeFilePicker filePicker;

        private ImportLayerCommand command;
        
        [SetUp]
        public void SetUp()
        {
            this.imageCodec = new();
            this.filePicker = new();
            this.command = new(imageCodec, filePicker);
        }

        [Test]
        public async Task ImportsFilesAsLayers()
        {
            IFile file1 = new InMemoryFile("file1");
            IFile file2 = new InMemoryFile("file2");

            Layer layer1 = new Layer { Name = file1.Name };
            Layer layer2 = new Layer { Name = file2.Name };

            SetupImageDecoder(file1, layer1);
            SetupImageDecoder(file2, layer2);
            filePicker.OpenFilesResult = new[] { file1, file2 };

            Image image = new Image();
            await command.ExecuteAsync(new Document(image));

            Assert.That(image.Layers, Is.EqualTo(new Layer[] { layer1, layer2 }).AsCollection);
        }

        private void SetupImageDecoder(IFile file, Layer layer)
        {
            Image image = new Image();
            image.Layers.Add(layer);
            imageCodec.SavedImages[file] = image;
        }
    }
}
