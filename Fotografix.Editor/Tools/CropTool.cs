using Fotografix.Editor.Commands;
using System;
using System.ComponentModel;
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
            image.PropertyChanged += Image_PropertyChanged;
            InitializeTracker();
        }

        public void Deactivated()
        {
            this.tracker = null;

            image.SetCropPreview(null);
            image.PropertyChanged -= Image_PropertyChanged;
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

        public async void Commit()
        {
            await image.DispatchAsync(new CropCommand(image, tracker.Rectangle));
        }

        private void Image_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Image.Size))
            {
                InitializeTracker();
            }
        }

        private void InitializeTracker()
        {
            Rectangle rect = new Rectangle(Point.Empty, image.Size);
            this.tracker = new RectangleTracker(rect) { HandleTolerance = 8 };
            tracker.RectangleChanged += Tracker_RectangleChanged;

            image.SetCropPreview(rect);
        }

        private void Tracker_RectangleChanged(object sender, EventArgs e)
        {
            image.SetCropPreview(tracker.Rectangle);
        }
    }
}
