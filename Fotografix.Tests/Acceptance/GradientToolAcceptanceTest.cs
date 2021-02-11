using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class GradientToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task DrawsLinearGradient()
        {
            await OpenImageAsync("flowers.jpg");

            var gradientControls = SelectTool<IGradientToolControls>("Gradient");
            gradientControls.StartColor = Color.Red;
            gradientControls.EndColor = Color.Green;

            AssertToolCursor(ToolCursor.Crosshair);

            DragAndReleasePointer(
                new Point(100, 100),
                new Point(300, 300)
            );

            await AssertImageAsync("gradient.png");
        }
    }
}
