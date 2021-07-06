using Fotografix.Editor.FileManagement;
using Fotografix.IO;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Editor.Layers
{
    [TestFixture]
    public class ImportLayerCommandTest
    {
        private Mock<IImageDecoder> imageDecoder;
        private Mock<IFilePicker> filePicker;
        private IEnumerable<FileFormat> fileFormats;

        private ImportLayerCommand command;
        
        [SetUp]
        public void SetUp()
        {
            this.imageDecoder = new();
            this.filePicker = new();
            this.fileFormats = new FileFormat[] { new FileFormat("Test", ".tst") };

            this.command = new(imageDecoder.Object, filePicker.Object);
        }

        [Test]
        public async Task ImportsFilesAsLayers()
        {
            IFile file1 = new InMemoryFile("file1");
            IFile file2 = new InMemoryFile("file2");

            Layer layer1 = new Layer { Name = file1.Name };
            Layer layer2 = new Layer { Name = file2.Name };

            imageDecoder.SetupGet(d => d.SupportedFileFormats).Returns(fileFormats);
            SetupImageDecoder(file1, layer1);
            SetupImageDecoder(file2, layer2);

            IEnumerable<IFile> files = new IFile[] { file1, file2 };
            filePicker.Setup(p => p.PickOpenFilesAsync(It.IsAny<IEnumerable<FileFormat>>())).Returns(Task.FromResult(files));

            Image image = new Image();
            await command.ExecuteAsync(new Document(image));

            filePicker.Verify(p => p.PickOpenFilesAsync(fileFormats));
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { layer1, layer2 }).AsCollection);
        }

        private void SetupImageDecoder(IFile file, Layer layer)
        {
            Image image = new Image();
            image.Layers.Add(layer);
            imageDecoder.Setup(d => d.ReadImageAsync(file)).Returns(Task.FromResult(image));
        }
    }
}
