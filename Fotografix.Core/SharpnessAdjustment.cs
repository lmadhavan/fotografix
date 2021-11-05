using Fotografix.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fotografix
{
    public sealed class SharpnessAdjustment : NotifyPropertyChangedBase, ISharpnessAdjustment, IDisposable
    {
        private readonly GaussianBlurEffect blurEffect;
        private readonly PixelShaderEffect unsharpMaskEffect;

        private float amount;
        private float radius;
        private float threshold;

        private float renderScale;
        private ICanvasImage source;

        public SharpnessAdjustment()
        {
            this.blurEffect = new GaussianBlurEffect { BorderMode = EffectBorderMode.Hard, Optimization = EffectOptimization.Quality };
            this.unsharpMaskEffect = ShaderFactory.CreatePixelShaderEffect("UnsharpMask");
            unsharpMaskEffect.Source2 = blurEffect;

            this.RenderScale = 1;
            this.Radius = 1;
        }

        public void Dispose()
        {
            unsharpMaskEffect.Dispose();
            blurEffect.Dispose();
        }

        public float Amount
        {
            get => amount;

            set
            {
                if (SetProperty(ref amount, value))
                {
                    unsharpMaskEffect.Properties["amount"] = value;
                }
            }
        }

        [DefaultValue(1f)]
        public float Radius
        {
            get => radius;

            set
            {
                if (SetProperty(ref radius, value))
                {
                    UpdateRadius();
                }
            }
        }

        public float Threshold
        {
            get => threshold;

            set
            {
                if (SetProperty(ref threshold, value))
                {
                    unsharpMaskEffect.Properties["threshold"] = value;
                }
            }
        }

        [JsonIgnore]
        public ICanvasImage Source
        {
            get => source;

            set
            {
                this.source = value;
                blurEffect.Source = value;
                unsharpMaskEffect.Source1 = value;
            }
        }

        [JsonIgnore]
        public ICanvasImage Output => amount == 0 ? source : unsharpMaskEffect;

        [JsonIgnore]
        public float RenderScale
        {
            get => renderScale;

            set
            {
                if (SetProperty(ref renderScale, value))
                {
                    UpdateRadius();
                }
            }
        }

        private void UpdateRadius()
        {
            // BlurAmount is the standard deviation of the Gaussian blur
            blurEffect.BlurAmount = renderScale * radius / 2;
        }
    }
}
