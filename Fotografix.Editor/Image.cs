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

        public Image(ICanvasResourceCreator resourceCreator, int width, int height)
            : this(new CanvasRenderTarget(resourceCreator, width, height, 96))
        {
        }

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

            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;
            adjustment.OutputChanged += OnAdjustmentOutputChanged;

            adjustments.Add(adjustment);
            RelinkAdjustments();
        }

        public void DeleteAdjustment(Adjustment adjustment)
        {
            if (adjustments.Remove(adjustment))
            {
                adjustment.PropertyChanged -= OnAdjustmentPropertyChanged;
                adjustment.OutputChanged -= OnAdjustmentOutputChanged;

                RelinkAdjustments();
            }
        }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Invalidate();
        }

        private void OnAdjustmentOutputChanged(object sender, EventArgs e)
        {
            RelinkAdjustments();
        }

        private bool relinking;

        private void RelinkAdjustments()
        {
            if (relinking)
            {
                return;
            }

            this.relinking = true;

            int n = adjustments.Count;

            if (n == 0)
            {
                this.output = bitmap;
            }
            else
            {
                adjustments[0].Input = bitmap;

                for (int i = 1; i < n; i++)
                {
                    adjustments[i].Input = adjustments[i - 1].Output;
                }

                this.output = adjustments[n - 1].Output;
            }

            Invalidate();
            this.relinking = false;
        }

        private void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}