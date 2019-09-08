using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fotografix.Editor
{
    public sealed class ImageEditorViewModel : IDisposable
    {
        private readonly Image image;

        public ImageEditorViewModel(Image image)
        {
            this.image = image;
        }

        public void Dispose()
        {
            image.Dispose();
        }

        public event EventHandler Invalidated;

        public int Width => image.Width;
        public int Height => image.Height;

        public IList<string> BlendModes { get; } = Enum.GetNames(typeof(BlendMode)).ToList();
        public int SelectedBlendModeIndex { get; set; }
        public BlendMode SelectedBlendMode => (BlendMode)SelectedBlendModeIndex;

        public void Draw(CanvasDrawingSession drawingSession)
        {
            image.Draw(drawingSession);
        }

        public void ApplyBlackAndWhiteAdjustment()
        {
            image.ApplyBlackAndWhiteAdjustment(SelectedBlendMode);
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}
