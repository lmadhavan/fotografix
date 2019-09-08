using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Editor
{
    internal sealed class BlackAndWhiteAdjustment : IDisposable
    {
        private readonly GrayscaleEffect grayscaleEffect;
        private readonly BlendEffect blendEffect;

        internal BlackAndWhiteAdjustment(ICanvasImage input, BlendMode blendMode)
        {
            this.grayscaleEffect = new GrayscaleEffect() { Source = input };

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
                    Background = input,
                    Mode = blendEffectMode
                };

                this.Output = blendEffect;
            }
        }

        public void Dispose()
        {
            blendEffect?.Dispose();
            grayscaleEffect?.Dispose();
        }

        internal ICanvasImage Output { get; }
    }
}