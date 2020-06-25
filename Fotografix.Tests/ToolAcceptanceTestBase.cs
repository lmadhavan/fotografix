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

        protected void PressAndDragPointer(PointF[] points)
        {
            Editor.PointerPressed(points[0]);

            for (int i = 1; i < points.Length; i++)
            {
                Editor.PointerMoved(points[i]);
            }
        }

        protected void ContinueDraggingAndReleasePointer(PointF[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Editor.PointerMoved(points[i]);
            }

            Editor.PointerReleased(points[points.Length - 1]);
        }
    }
}