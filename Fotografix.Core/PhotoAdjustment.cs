using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Newtonsoft.Json;
using System;
using Windows.Graphics.Effects;

namespace Fotografix
{
    public sealed class PhotoAdjustment : NotifyPropertyChangedBase, IDisposable
    {
        private readonly GammaTransferEffect transferEffect;
        private readonly HighlightsAndShadowsEffect highlightsAndShadowsEffect;
        private readonly ContrastEffect contrastEffect;
        private readonly TemperatureAndTintEffect temperatureAndTintEffect;

        public PhotoAdjustment()
        {
            this.transferEffect = new GammaTransferEffect();
            this.highlightsAndShadowsEffect = new HighlightsAndShadowsEffect { Source = transferEffect };
            this.contrastEffect = new ContrastEffect { Source = highlightsAndShadowsEffect };
            this.temperatureAndTintEffect = new TemperatureAndTintEffect { Source = contrastEffect };
        }

        public void Dispose()
        {
            temperatureAndTintEffect.Dispose();
            contrastEffect.Dispose();
            highlightsAndShadowsEffect.Dispose();
            transferEffect.Dispose();
        }

        [JsonIgnore]
        public IGraphicsEffectSource Source
        {
            get => transferEffect.Source;
            set => transferEffect.Source = value;
        }

        [JsonIgnore]
        public ICanvasImage Output => temperatureAndTintEffect;

        #region Light

        private float exposure;
        private float whites;
        private float blacks;

        public float Exposure
        {
            get => exposure;

            set
            {
                if (SetProperty(ref exposure, value))
                {
                    UpdateTransferEffect();
                }
            }
        }

        public float Contrast
        {
            get => contrastEffect.Contrast;

            set
            {
                if (contrastEffect.Contrast != value)
                {
                    contrastEffect.Contrast = value;
                    RaisePropertyChanged();
                }
            }
        }

        public float Whites
        {
            get => whites;

            set
            {
                if (SetProperty(ref whites, value))
                {
                    UpdateTransferEffect();
                }
            }
        }

        public float Blacks
        {
            get => blacks;

            set
            {
                if (SetProperty(ref blacks, value))
                {
                    UpdateTransferEffect();
                }
            }
        }

        private void UpdateTransferEffect()
        {
            // Scale white/black point adjustments so they are at a similar level as highlights/shadows adjustments
            var wp = 1 - whites / 4;
            var bp = 0 - blacks / 4;

            /*
             * Gamma transfer effect does pow(input, exponent) * amplitude + offset
             * We want ((input * 2^exposure) - bp) / (wp - bp)
             */
            var amplitude = Math.Pow(2, exposure) / (wp - bp);
            var offset = -bp / (wp - bp);

            transferEffect.RedAmplitude = transferEffect.GreenAmplitude = transferEffect.BlueAmplitude = (float)amplitude;
            transferEffect.RedOffset = transferEffect.GreenOffset = transferEffect.BlueOffset = offset;
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
