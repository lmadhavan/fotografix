using Fotografix.Editor.Crop;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class CropTool : ITool, ICropToolControls
    {
        private Image image;
        private RectangleTracker tracker;

        public string Name => "Crop";

        public ToolCursor Cursor => tracker?.Cursor ?? ToolCursor.Disabled;

        public void Activated(Image image)
        {
            this.image = image;
            Rectangle rect = new Rectangle(Point.Empty, image.Size);
            this.tracker = new RectangleTracker(rect) { HandleTolerance = 8 };
        }

        public void Deactivated()
        {
            this.tracker = null;
            this.image = null;
        }

        public void PointerPressed(PointerState p)
        {
            tracker.PointerPressed(p);
        }

        public void PointerMoved(PointerState p)
        {
            tracker.PointerMoved(p);
        }

        public void PointerReleased(PointerState p)
        {
            tracker.PointerReleased(p);
        }

        public void Commit()
        {
            image.Dispatch(new CropCommand(image, tracker.Rectangle));
        }
    }
}
