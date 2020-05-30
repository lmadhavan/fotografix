using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Win2D.Composition
{
    internal sealed class CompositeEffectNode : IDisposable
    {
        private readonly CompositeEffect compositeEffect;

        internal CompositeEffectNode()
        {
            this.compositeEffect = new CompositeEffect();
            // reserve background and foreground slots for the effect, actual sources are set later
            compositeEffect.Sources.Add(null);
            compositeEffect.Sources.Add(null);
        }

        public void Dispose()
        {
            compositeEffect.Dispose();
        }

        public ICanvasImage ResolveOutput(ICanvasImage foreground, ICanvasImage background)
        {
            compositeEffect.Sources[0] = background;
            compositeEffect.Sources[1] = foreground;
            return compositeEffect;
        }
    }
}
