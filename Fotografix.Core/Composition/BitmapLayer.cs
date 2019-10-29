using Microsoft.Graphics.Canvas;

namespace Fotografix.Composition
{
    public sealed class BitmapLayer : Layer
    {
        public BitmapLayer(CanvasBitmap bitmap)
        {
            this.Content = bitmap;
        }
    }
}
