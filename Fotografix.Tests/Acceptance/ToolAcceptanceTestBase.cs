﻿using Fotografix.Editor;
using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Linq;

namespace Fotografix.Tests.Acceptance
{
    public abstract class ToolAcceptanceTestBase : AcceptanceTestBase
    {
        private ITool ActiveTool => Editor.ActiveTool;

        protected void SelectTool(string name)
        {
            ITool tool = Editor.Tools.FirstOrDefault(t => t.Name == name);

            if (tool == null)
            {
                Assert.Fail($"Could not find a tool named {name}");
            }
            
            Editor.ActiveTool = tool;
        }

        protected TControls SelectTool<TControls>(string name)
        {
            SelectTool(name);
            return (TControls)ActiveTool;
        }

        protected void AssertToolCursor(ToolCursor expected)
        {
            Assert.AreEqual(expected, ActiveTool.Cursor);
        }

        protected void DragAndReleasePointer(Point start, params Point[] points)
        {
            PressAndDragPointer(start);
            ContinueDraggingAndReleasePointer(points);
        }

        protected void PressAndDragPointer(Point start, params Point[] points)
        {
            ActiveTool.PointerPressed(new PointerState(start));

            foreach (Point pt in points)
            {
                ActiveTool.PointerMoved(new PointerState(pt));
            }
        }

        protected void ContinueDraggingAndReleasePointer(params Point[] points)
        {
            foreach (Point pt in points)
            {
                ActiveTool.PointerMoved(new PointerState(pt));
            }

            ActiveTool.PointerReleased(new PointerState(points.Last()));
        }
    }
}