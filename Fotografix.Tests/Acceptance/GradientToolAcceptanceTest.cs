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
            ConfigureGradientTool(startColor: Color.Red, endColor: Color.Green);

            AssertToolCursor(ToolCursor.Crosshair);

            PressAndDragPointer(
                new Point(100, 100),
                new Point(200, 200)
            );

            await AssertImageAsync("gradient_preview.png");

            ContinueDraggingAndReleasePointer(new Point(300, 300));

            await AssertImageAsync("gradient.png");
        }

        private void ConfigureGradientTool(Color startColor, Color endColor)
        {
            var gradientControls = SelectTool<IGradientToolControls>("Gradient");
            gradientControls.StartColor = startColor;
            gradientControls.EndColor = endColor;
        }
    }
}
