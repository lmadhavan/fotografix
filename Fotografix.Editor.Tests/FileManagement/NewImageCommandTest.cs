using Moq;
using NUnit.Framework;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    [TestFixture]
    public class NewImageCommandTest
    {
        private Mock<IDialog<NewImageParameters>> newImageDialog;
        private NewImageCommand command;

        [SetUp]
        public void SetUp()
        {
            this.newImageDialog = new();
            this.command = new(newImageDialog.Object);
        }

        [Test]
        public async Task CreatesNewDocumentInWorkspace()
        {
            newImageDialog.Setup(d => d.ShowAsync(It.IsAny<NewImageParameters>()))
                .Callback((NewImageParameters p) =>
                {
                    p.Width = 200;
                    p.Height = 100;
                })
                .Returns(Task.FromResult(true));


            Workspace workspace = new();
            await command.ExecuteAsync(workspace);

            Assert.That(workspace.Documents, Has.Count.EqualTo(1));

            Image image = workspace.Documents.First().Image;
            Assert.That(image.Size, Is.EqualTo(new Size(200, 100)));
            Assert.That(image.Layers, Has.Count.EqualTo(1));
        }
    }
}
