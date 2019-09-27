using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Editor
{
    public sealed class Image : IDisposable
    {
        private readonly CanvasBitmap bitmap;
        private readonly ObservableCollection<Adjustment> adjustments;
        private ICanvasImage output;

        public Image(CanvasBitmap bitmap)
        {
            this.bitmap = bitmap;
            this.adjustments = new ObservableCollection<Adjustment>();
            this.Adjustments = new ReadOnlyObservableCollection<Adjustment>(adjustments);
            this.output = bitmap;
        }

        public void Dispose()
        {
            foreach (Adjustment adjustment in adjustments)
            {
                adjustment.OutputChanged -= OnAdjustmentOutputChanged;
                adjustment.PropertyChanged -= OnAdjustmentPropertyChanged;
            }

            bitmap.Dispose();
        }

        public event EventHandler Invalidated;

        public int Width => (int)bitmap.SizeInPixels.Width;
        public int Height => (int)bitmap.SizeInPixels.Height;

        public ReadOnlyObservableCollection<Adjustment> Adjustments { get; }

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

            Invalidate();
            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;
            adjustment.OutputChanged += OnAdjustmentOutputChanged;
        }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Invalidate();
        }

        private void OnAdjustmentOutputChanged(object sender, EventArgs e)
        {
            Adjustment adjustment = (Adjustment)sender;

            int i = adjustments.IndexOf(adjustment);
            if (i == adjustments.Count - 1)
            {
                this.output = adjustment.Output;
            }
            else
            {
                adjustments[i + 1].Input = adjustment.Output;
            }
        }

        private void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}