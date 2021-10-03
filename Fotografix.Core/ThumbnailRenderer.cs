using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace Fotografix
{
    public class ThumbnailRenderer
    {
        private readonly int thumbnailSize;

        public ThumbnailRenderer(int thumbnailSize)
        {
            this.thumbnailSize = thumbnailSize;
        }

        public CanvasBitmap RenderThumbnail(ICanvasImage image, ICanvasResourceCreatorWithDpi resourceCreator)
        {
            var originalBounds = image.GetBounds(resourceCreator);
            var thumbnailSize = GetThumbnailSize(new Size(originalBounds.Width, originalBounds.Height));
            var rt = new CanvasRenderTarget(resourceCreator, thumbnailSize);

            using (var ds = rt.CreateDrawingSession())
            {
                ds.DrawImage(image, rt.Bounds, originalBounds);
            }

            return rt;
        }

        public Size GetThumbnailSize(Size originalSize)
        {
            if (originalSize.Width > originalSize.Height)
            {
                return new Size(thumbnailSize, thumbnailSize * originalSize.Height / originalSize.Width);
            }
            else
            {
                return new Size(thumbnailSize * originalSize.Width / originalSize.Height, thumbnailSize);
            }
        }
    }
}
