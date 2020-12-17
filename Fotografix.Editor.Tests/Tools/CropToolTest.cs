using Fotografix.Editor.Crop;
using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class CropToolTest
    {
        [Test]
        public void CommitsCropOperation()
        {
            Mock<ICommandDispatcher> commandDispatcher = new Mock<ICommandDispatcher>();

            Size size = new Size(10, 10);
            Image image = new Image(size);
            image.SetCommandDispatcher(commandDispatcher.Object);

            CropTool tool = new CropTool();
            tool.Activated(image);
            tool.Commit();

            commandDispatcher.Verify(d => d.Dispatch(new CropCommand(image, new Rectangle(Point.Empty, size))));
        }
    }
}
