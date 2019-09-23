using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Editor
{
    public sealed class Image : IDisposable
    {
        private readonly CanvasBitmap bitmap;
        private readonly List<Adjustment> adjustments;
        private ICanvasImage output;

        public Image(CanvasBitmap bitmap)
        {
            this.bitmap = bitmap;
            this.adjustments = new List<Adjustment>();
            this.output = bitmap;
        }

        public void Dispose()
        {
            foreach (Adjustment adjustment in adjustments)
            {
                adjustment.Dispose();
            }

            bitmap.Dispose();
        }

        public int Width => (int)bitmap.SizeInPixels.Width;
        public int Height => (int)bitmap.SizeInPixels.Height;

        public IReadOnlyList<Adjustment> Adjustments => adjustments;

        public static async Task<Image> LoadAsync(StorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                var bitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
                return new Image(bitmap);
            }
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            drawingSession.DrawImage(output);
        }

        public void AddAdjustment(Adjustment adjustment)
        {
            if (adjustment.Input != null)
            {
                throw new ArgumentException("Adjustment is already attached to another object");
            }

            adjustment.Input = output;
            adjustments.Add(adjustment);
            this.output = adjustment.Output;
        }
    }
}