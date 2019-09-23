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
            foreach (Adjustment adjustment in image.Adjustments)
            {
                adjustment.Dispose();
            }

            image.Dispose();
        }

        public event EventHandler Invalidated;

        public int Width => image.Width;
        public int Height => image.Height;

        public IList<string> BlendModes { get; } = Enum.GetNames(typeof(BlendMode)).ToList();
        public int SelectedBlendModeIndex { get; set; }
        public BlendMode SelectedBlendMode => (BlendMode)SelectedBlendModeIndex;

        public float Shadows { get; set; } = 0;
        public float Highlights { get; set; } = 0;
        public float Clarity { get; set; } = 0;

        public void Draw(CanvasDrawingSession drawingSession)
        {
            image.Draw(drawingSession);
        }

        public void AddBlackAndWhiteAdjustment()
        {
            image.AddAdjustment(new BlackAndWhiteAdjustment(SelectedBlendMode));
            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        public void AddShadowsHighlightsAdjustment()
        {
            image.AddAdjustment(new ShadowsHighlightsAdjustment(Shadows, Highlights, Clarity));
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}
