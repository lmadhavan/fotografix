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

            PressAndDragPointer(new Point[] {
                new Point(100, 100),
                new Point(200, 200)
            });

            await AssertImageAsync("gradient_preview.png");

            ContinueDraggingAndReleasePointer(new Point[]
            {
                new Point(300, 300)
            });

            await AssertImageAsync("gradient.png");

            Undo();

            await AssertImageAsync("flowers.jpg");
        }

        private void ConfigureGradientTool(Color startColor, Color endColor)
        {
            IGradientToolSettings settings = SelectTool<IGradientToolSettings>("Gradient");
            settings.StartColor = startColor;
            settings.EndColor = endColor;
        }
    }
}
