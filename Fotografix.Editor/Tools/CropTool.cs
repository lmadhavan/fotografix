using System;
using System.ComponentModel;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class CropTool : ITool, ICropToolControls
    {
        private readonly IDocumentCommand cropCommand;

        private Document document;
        private Image image;
        private RectangleTracker tracker;

        public CropTool(IDocumentCommand cropCommand)
        {
            this.cropCommand = cropCommand;
        }

        public string Name => "Crop";
        public ToolCursor Cursor => tracker?.Cursor ?? ToolCursor.Disabled;

        public void Activated(Document document)
        {
            this.document = document;
            this.image = document.Image;
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
            await cropCommand.ExecuteAsync(document);
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
