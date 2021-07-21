using NUnit.Framework;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    [TestFixture]
    public class NewImageCommandTest
    {
        [Test]
        public async Task CreatesNewDocumentInWorkspace()
        {
            static bool NewImageDialog(NewImageParameters p)
            {
                p.Width = 200;
                p.Height = 100;
                return true;
            }

            Workspace workspace = new();
            NewImageCommand command = new(new Dialog<NewImageParameters>(NewImageDialog));
            await command.ExecuteAsync(workspace);

            Assert.That(workspace.Documents, Has.Count.EqualTo(1));

            Image image = workspace.Documents.First().Image;
            Assert.That(image.Size, Is.EqualTo(new Size(200, 100)));
            Assert.That(image.Layers, Has.Count.EqualTo(1));
        }
    }
}
