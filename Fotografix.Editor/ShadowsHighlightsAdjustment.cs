using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Graphics.Effects;

namespace Fotografix.Editor
{
    public sealed class ShadowsHighlightsAdjustment : Adjustment
    {
        private readonly HighlightsAndShadowsEffect effect;

        public ShadowsHighlightsAdjustment(float shadows, float highlights, float clarity)
        {
            this.effect = new HighlightsAndShadowsEffect()
            {
                Shadows = shadows,
                Highlights = highlights,
                Clarity = clarity
            };
        }

        public override void Dispose()
        {
            effect.Dispose();
        }

        internal override IGraphicsEffectSource Input
        {
            get
            {
                return effect.Source;
            }

            set
            {
                effect.Source = value;
            }
        }

        internal override ICanvasImage Output => effect;
    }
}