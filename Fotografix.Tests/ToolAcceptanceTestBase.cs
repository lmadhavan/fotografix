using Fotografix.Editor.Testing;
using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Fotografix.Tests
{
    public abstract class ToolAcceptanceTestBase : AcceptanceTestBase
    {
        protected void SelectTool(string name)
        {
            Editor.ActiveTool = name;
        }

        protected TSettings SelectTool<TSettings>(string name)
        {
            SelectTool(name);
            return (TSettings)Editor.ToolSettings;
        }

        protected void AssertToolCursor(ToolCursor expected)
        {
            Assert.AreEqual(expected, Editor.ToolCursor);
        }

        protected void PressAndDragPointer(Point[] points)
        {
            Editor.PointerPressed(new FakePointerEvent(points[0]));

            for (int i = 1; i < points.Length; i++)
            {
                Editor.PointerMoved(new FakePointerEvent(points[i]));
            }
        }

        protected void ContinueDraggingAndReleasePointer(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Editor.PointerMoved(new FakePointerEvent(points[i]));
            }

            Editor.PointerReleased(new FakePointerEvent(points[points.Length - 1]));
        }
    }
}