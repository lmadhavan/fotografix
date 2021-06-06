using Fotografix.Editor;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Drawing;
using Windows.Foundation;
using Windows.UI;

namespace Fotografix.Win2D.Composition
{
    internal sealed class SelectionNode : IDrawableNode
    {
        private const float DashWidth = 4;

        private readonly Image image;
        private readonly Viewport viewport;
        private readonly CanvasStrokeStyle dashStroke;

        public SelectionNode(Image image, Viewport viewport)
        {
            this.image = image;
            this.viewport = viewport;

            this.dashStroke = new CanvasStrokeStyle
            {
                DashCap = CanvasCapStyle.Flat,
                CustomDashStyle = new float[] { DashWidth, DashWidth }
            };
        }

        public void Dispose()
        {
            dashStroke.Dispose();
        }

        public void Draw(CanvasDrawingSession ds, Rect imageBounds)
        {
            var selection = image.Selection;

            if (selection != Rectangle.Empty)
            {
                Rect rect = viewport.TransformImageToViewport(selection).ToWindowsRect();
                ds.DrawRectangle(rect, Colors.Black, 1);
                ds.DrawRectangle(rect, Colors.White, 1, dashStroke);
            }
        }
    }
}