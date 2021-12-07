using Fotografix.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Newtonsoft.Json;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.Effects;

namespace Fotografix
{
    public sealed class PhotoAdjustment : NotifyPropertyChangedBase, IDisposable, IPhotoAdjustment
    {
        private readonly CropEffect cropEffect;
        private readonly Transform2DEffect transformEffect;
        private readonly GammaTransferEffect transferEffect;
        private readonly HighlightsAndShadowsEffect highlightsAndShadowsEffect;
        private readonly ContrastEffect contrastEffect;
        private readonly TemperatureAndTintEffect temperatureAndTintEffect;
        private readonly RgbToHueEffect rgbToHueEffect;
        private readonly PixelShaderEffect hslAdjustmentEffect;
        private readonly HueToRgbEffect hueToRgbEffect;
        private readonly SharpnessAdjustment sharpnessAdjustment;

        public PhotoAdjustment()
        {
            this.cropEffect = new CropEffect { BorderMode = EffectBorderMode.Hard };
            this.transformEffect = new Transform2DEffect();
            this.transferEffect = new GammaTransferEffect { Source = transformEffect };
            this.highlightsAndShadowsEffect = new HighlightsAndShadowsEffect { Source = transferEffect };
            this.contrastEffect = new ContrastEffect { Source = highlightsAndShadowsEffect };
            this.temperatureAndTintEffect = new TemperatureAndTintEffect { Source = contrastEffect };
            this.rgbToHueEffect = new RgbToHueEffect { Source = temperatureAndTintEffect, OutputColorSpace = EffectHueColorSpace.Hsl };

            this.hslAdjustmentEffect = ShaderFactory.CreatePixelShaderEffect("HSLAdjustment");
            hslAdjustmentEffect.Source1 = rgbToHueEffect;

            this.hueToRgbEffect = new HueToRgbEffect { Source = hslAdjustmentEffect, SourceColorSpace = EffectHueColorSpace.Hsl };
            this.sharpnessAdjustment = new SharpnessAdjustment { Source = hueToRgbEffect };

            this.PropertyChanged += (s, e) => RaiseChanged();
            sharpnessAdjustment.PropertyChanged += (s, e) => RaiseChanged();
            this.ColorRanges = new ColorRangeAdjustment();
        }

        public void Dispose()
        {
            UnsubscribeColorRanges();

            sharpnessAdjustment.Dispose();
            hueToRgbEffect.Dispose();
            hslAdjustmentEffect.Dispose();
            rgbToHueEffect.Dispose();
            temperatureAndTintEffect.Dispose();
            contrastEffect.Dispose();
            highlightsAndShadowsEffect.Dispose();
            transferEffect.Dispose();
            transformEffect.Dispose();
            cropEffect.Dispose();
        }

        public event EventHandler Changed;

        #region Rendering parameters

        private bool enabled = true;
        private float renderScale = 1;

        [JsonIgnore]
        public IGraphicsEffectSource Source
        {
            get => cropEffect.Source;

            set
            {
                cropEffect.Source = value;
                UpdateTransform();
            }
        }

        [JsonIgnore]
        public ICanvasImage Output => enabled ? sharpnessAdjustment.Output : transformEffect;

        [JsonIgnore]
        public bool Enabled
        {
            get => enabled;
            set => SetProperty(ref enabled, value);
        }

        [JsonIgnore]
        public float RenderScale
        {
            get => renderScale;

            set
            {
                value = Math.Min(1, value);

                if (SetProperty(ref renderScale, value))
                {
                    UpdateTransform();
                    sharpnessAdjustment.RenderScale = value;

                    // scale radius-based adjustments to match the input scale
                    highlightsAndShadowsEffect.MaskBlurAmount = 1.25f * renderScale;
                }
            }
        }

        private void UpdateTransform()
        {
            var matrix = Matrix3x2.CreateScale(renderScale);

            if (crop.HasValue)
            {
                matrix = Matrix3x2.CreateTranslation((float)-crop.Value.X, (float)-crop.Value.Y) * matrix;
                transformEffect.Source = cropEffect;
            }
            else
            {
                transformEffect.Source = cropEffect.Source;
            }

            transformEffect.TransformMatrix = matrix;
        }

        public Size GetOutputSize(ICanvasResourceCreator resourceCreator)
        {
            var bounds = transferEffect.GetBounds(resourceCreator);
            return new Size(bounds.Width, bounds.Height);
        }

        #endregion

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

        private bool blackAndWhite;
        private float vibrance;
        private float saturation;
        private ColorRangeAdjustment colorRanges;

        public bool BlackAndWhite
        {
            get => blackAndWhite;

            set
            {
                if (SetProperty(ref blackAndWhite, value))
                {
                    Vibrance = 0;
                    Saturation = blackAndWhite ? -1 : 0;
                    ColorRanges.SaturationView.Reset();
                    ColorRanges.LuminanceView.Reset();
                }
            }
        }

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

        public float Vibrance
        {
            get => vibrance;

            set
            {
                if (SetProperty(ref vibrance, value))
                {
                    hslAdjustmentEffect.Properties["vibrance"] = vibrance;
                }
            }
        }

        public float Saturation
        {
            get => saturation;

            set
            {
                if (SetProperty(ref saturation, value))
                {
                    hslAdjustmentEffect.Properties["saturation"] = saturation;
                }
            }
        }

        public ColorRangeAdjustment ColorRanges
        {
            get => colorRanges;

            set
            {
                UnsubscribeColorRanges();

                this.colorRanges = value ?? throw new ArgumentNullException();
                colorRanges.Changed += ColorRanges_Changed;

                UpdateHslAdjustment();
            }
        }

        private void UnsubscribeColorRanges()
        {
            if (colorRanges != null)
            {
                colorRanges.Changed -= ColorRanges_Changed;
            }
        }

        private void ColorRanges_Changed(object sender, EventArgs e)
        {
            UpdateHslAdjustment();
        }

        private void UpdateHslAdjustment()
        {
            hslAdjustmentEffect.Properties["hsl"] = colorRanges.vectors;
            RaiseChanged();
        }

        #endregion

        #region Detail

        public float Clarity
        {
            get => highlightsAndShadowsEffect.Clarity;

            set
            {
                if (highlightsAndShadowsEffect.Clarity != value)
                {
                    highlightsAndShadowsEffect.Clarity = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ISharpnessAdjustment Sharpness => sharpnessAdjustment;

        #endregion

        #region Crop

        private CropRect? crop;

        public CropRect? Crop
        {
            get => crop;

            set
            {
                if (SetProperty(ref crop, value))
                {
                    if (crop.HasValue)
                    {
                        cropEffect.SourceRectangle = crop.Value;
                    }

                    UpdateTransform();
                }
            }
        }

        #endregion

        #region Serialization

        private static readonly string EmptyJson;

        static PhotoAdjustment()
        {
            using (var adjustment = new PhotoAdjustment())
            {
                EmptyJson = adjustment.Serialize();
            }
        }

        public string Serialize()
        {
            var result = JsonConvert.SerializeObject(this, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            return result == EmptyJson ? "" : result;
        }

        public static PhotoAdjustment Deserialize(string str)
        {
            return (PhotoAdjustment)JsonConvert.DeserializeObject(str, typeof(PhotoAdjustment));
        }

        #endregion

        private void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
