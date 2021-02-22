using Fotografix.Drawing;
using System.Drawing;

namespace Fotografix.Tests
{
    internal static class TestDrawables
    {
        internal static readonly IDrawable BrushStroke = new BrushStroke(
            points: new Point[] { new Point(75, 100), new Point(250, 350) },
            size: 5,
            color: Color.White
        );
    }
}
