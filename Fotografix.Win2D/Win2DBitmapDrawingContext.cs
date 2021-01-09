using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;

namespace Fotografix.Win2D
{
    internal sealed class Win2DBitmapDrawingContext : IDrawingContext
    {
        private readonly Win2DBitmap bitmap;
        private readonly IDrawingContext drawingContext;

        public Win2DBitmapDrawingContext(Bitmap source)
        {
            this.bitmap = new Win2DBitmap(source, CanvasDevice.GetSharedDevice());
            this.drawingContext = bitmap.CreateDrawingContext();
        }

        public void Dispose()
        {
            drawingContext.Dispose();
            bitmap.UpdateSource();
            bitmap.Dispose();
        }

        public void Draw(BrushStroke brushStroke)
        {
            drawingContext.Draw(brushStroke);
        }

        public void Draw(LinearGradient gradient)
        {
            drawingContext.Draw(gradient);
        }
    }
}
