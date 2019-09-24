using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.Graphics.Effects;

namespace Fotografix.Editor.Adjustments
{
    public sealed class BlackAndWhiteAdjustment : Adjustment, IDisposable
    {
        private readonly GrayscaleEffect grayscaleEffect;
        private readonly BlendEffect blendEffect;

        public BlackAndWhiteAdjustment(BlendMode blendMode = BlendMode.Normal)
        {
            this.Name = "Black & White";
            this.grayscaleEffect = new GrayscaleEffect();

            if (blendMode == BlendMode.Normal)
            {
                this.Output = grayscaleEffect;
            }
            else
            {
                BlendEffectMode blendEffectMode = Enum.Parse<BlendEffectMode>(Enum.GetName(typeof(BlendMode), blendMode));

                this.blendEffect = new BlendEffect()
                {
                    Foreground = grayscaleEffect,
                    Mode = blendEffectMode
                };

                this.Output = blendEffect;
            }
        }

        public override void Dispose()
        {
            blendEffect?.Dispose();
            grayscaleEffect?.Dispose();
        }

        internal override IGraphicsEffectSource Input
        {
            get
            {
                return grayscaleEffect.Source;
            }

            set
            {
                grayscaleEffect.Source = value;

                if (blendEffect != null)
                {
                    blendEffect.Background = value;
                }
            }
        }

        internal override ICanvasImage Output { get; }
    }
}