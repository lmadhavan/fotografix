using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Win2D.Composition
{
    internal sealed class BlendEffectNode : IDisposable
    {
        private readonly BlendEffect blendEffect;

        internal BlendEffectNode()
        {
            this.blendEffect = new BlendEffect();
        }

        public void Dispose()
        {
            blendEffect.Dispose();
        }

        public ICanvasImage Blend(ICanvasImage foreground, ICanvasImage background, BlendMode blendMode)
        {
            blendEffect.Mode = Enum.Parse<BlendEffectMode>(Enum.GetName(typeof(BlendMode), blendMode));
            blendEffect.Foreground = foreground;
            blendEffect.Background = background;
            return blendEffect;
        }
    }
}
