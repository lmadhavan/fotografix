using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix
{
    public sealed class PhotoAdjustment : NotifyPropertyChangedBase, IDisposable
    {
        private readonly ExposureEffect exposureEffect;

        public PhotoAdjustment()
        {
            this.exposureEffect = new ExposureEffect();
        }

        public void Dispose()
        {
            exposureEffect.Dispose();
        }

        public void Render(CanvasDrawingSession ds, ICanvasImage image)
        {
            exposureEffect.Source = image;
            ds.DrawImage(exposureEffect);
        }

        public float Exposure
        {
            get => exposureEffect.Exposure;
            
            set
            {
                if (exposureEffect.Exposure != value)
                {
                    exposureEffect.Exposure = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
