using Fotografix.Editor;
using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Linq;

namespace Fotografix.Tests
{
    public abstract class ToolAcceptanceTestBase : AcceptanceTestBase
    {
        private ITool ActiveTool => Editor.ActiveTool;

        protected void SelectTool(string name)
        {
            Editor.ActiveTool = Editor.Tools.First(tool => tool.Name == name);
        }

        protected TSettings SelectTool<TSettings>(string name)
        {
            SelectTool(name);
            return (TSettings)ActiveTool.Settings;
        }

        protected void AssertToolCursor(ToolCursor expected)
        {
            Assert.AreEqual(expected, ActiveTool.Cursor);
        }

        protected void PressAndDragPointer(Point[] points)
        {
            ActiveTool.PointerPressed(new PointerState(points[0]));

            for (int i = 1; i < points.Length; i++)
            {
                ActiveTool.PointerMoved(new PointerState(points[i]));
            }
        }

        protected void ContinueDraggingAndReleasePointer(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                ActiveTool.PointerMoved(new PointerState(points[i]));
            }

            ActiveTool.PointerReleased(new PointerState(points[points.Length - 1]));
        }
    }
}