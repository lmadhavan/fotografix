using Microsoft.Graphics.Canvas;

namespace Fotografix.Editor
{
    public sealed class BitmapLayer : Layer
    {
        public BitmapLayer(CanvasBitmap bitmap)
        {
            this.Content = bitmap;
        }
    }
}
