using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix
{
    public sealed class PhotoAdjustment : NotifyPropertyChangedBase, IDisposable
    {
        private readonly ExposureEffect exposureEffect;
        private readonly HighlightsAndShadowsEffect highlightsAndShadowsEffect;
        private readonly TemperatureAndTintEffect temperatureAndTintEffect;

        public PhotoAdjustment()
        {
            this.exposureEffect = new ExposureEffect();
            this.highlightsAndShadowsEffect = new HighlightsAndShadowsEffect { Source = exposureEffect };
            this.temperatureAndTintEffect = new TemperatureAndTintEffect { Source = highlightsAndShadowsEffect };
        }

        public void Dispose()
        {
            temperatureAndTintEffect.Dispose();
            highlightsAndShadowsEffect.Dispose();
            exposureEffect.Dispose();
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

        public float Highlights
        {
            get => highlightsAndShadowsEffect.Highlights;

            set
            {
                if (highlightsAndShadowsEffect.Highlights != value)
                {
                    highlightsAndShadowsEffect.Highlights = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float Shadows
        {
            get => highlightsAndShadowsEffect.Shadows;

            set
            {
                if (highlightsAndShadowsEffect.Shadows != value)
                {
                    highlightsAndShadowsEffect.Shadows = value;
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
