using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class SelectionToolTest
    {
        private SelectionTool tool;
        private Image image;

        [SetUp]
        public void SetUp()
        {
            this.tool = new SelectionTool();
            this.image = new Image(new Size(100, 100));
        }

        [Test]
        public void SelectsRectangleWhenPointerIsDragged()
        {
            tool.Activated(image);

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Crosshair));

            tool.PointerPressed(new(10, 10));
            tool.PointerMoved(new(50, 50));
            tool.PointerReleased(new(50, 50));

            var selection = image.GetSelection();
            Assert.IsTrue(selection.HasValue);
            Assert.That(selection.Value, Is.EqualTo(Rectangle.FromLTRB(10, 10, 50, 50)));
        }

        [Test]
        public void MovesExistingSelectionWhenPointerIsDraggedInside()
        {
            tool.Activated(image);
            image.SetSelection(new Rectangle(25, 25, 50, 50));

            tool.PointerMoved(new(30, 30));

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Move));

            tool.PointerPressed(new(30, 30));
            tool.PointerMoved(new(40, 40));

            var selection = image.GetSelection();
            Assert.That(selection.Value, Is.EqualTo(new Rectangle(35, 35, 50, 50)));
        }

        [Test]
        public void ClearsExistingSelectionWhenPointerIsClickedOutside()
        {
            tool.Activated(image);
            image.SetSelection(new Rectangle(25, 25, 50, 50));

            tool.PointerPressed(new(10, 10));
            tool.PointerReleased(new(10, 10));

            var selection = image.GetSelection();
            Assert.That(selection.HasValue, Is.False);
        }

        [Test]
        public void IgnoresMovementWhenPointerNotPressed()
        {
            tool.Activated(image);
            tool.PointerMoved(new(50, 50));

            var selection = image.GetSelection();
            Assert.IsFalse(selection.HasValue);
        }

        [Test]
        public void IgnoresMovementAfterPointerReleased()
        {
            tool.Activated(image);
            tool.PointerPressed(new(10, 10));
            tool.PointerMoved(new(50, 50));
            tool.PointerReleased(new(50, 50));
            tool.PointerMoved(new(100, 100));

            var selection = image.GetSelection();
            Assert.That(selection.Value, Is.EqualTo(Rectangle.FromLTRB(10, 10, 50, 50)));
        }
    }
}
