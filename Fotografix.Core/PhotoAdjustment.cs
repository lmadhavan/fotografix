using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix
{
    public sealed class PhotoAdjustment : NotifyPropertyChangedBase, IDisposable
    {
        private readonly ExposureEffect exposureEffect;
        private readonly TemperatureAndTintEffect temperatureAndTintEffect;

        public PhotoAdjustment()
        {
            this.exposureEffect = new ExposureEffect();
            this.temperatureAndTintEffect = new TemperatureAndTintEffect { Source = exposureEffect };
        }

        public void Dispose()
        {
            exposureEffect.Dispose();
            temperatureAndTintEffect.Dispose();
        }

        public void Render(CanvasDrawingSession ds, ICanvasImage image)
        {
            var firstEffect = exposureEffect;
            var lastEffect = temperatureAndTintEffect;

            firstEffect.Source = image;
            ds.DrawImage(lastEffect);
        }

        #region Light

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

        #endregion

        #region Color

        public float Temperature
        {
            get => temperatureAndTintEffect.Temperature;

            set
            {
                if (temperatureAndTintEffect.Temperature != value)
                {
                    temperatureAndTintEffect.Temperature = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float Tint
        {
            get => temperatureAndTintEffect.Tint;

            set
            {
                if (temperatureAndTintEffect.Tint != value)
                {
                    temperatureAndTintEffect.Tint = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion
    }
}
