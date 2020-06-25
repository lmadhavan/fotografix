using Fotografix.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests
{
    [TestClass]
    public class HandToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task RequestsViewportScroll()
        {
            await OpenImageAsync("flowers.jpg");
            SelectTool("Hand");

            ViewportScrollRequestedEventArgs eventArgs = null;
            Editor.ViewportScrollRequested += (s, e) => eventArgs = e;

            PressAndDragPointer(new PointF[] {
                new PointF(10, 10),
                new PointF(20, 20)
            });

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(new Point(-10, -10), eventArgs.ScrollDelta);
        }
    }
}
