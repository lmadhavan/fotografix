using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Editor.Adjustments
{
    public sealed class ShadowsHighlightsAdjustment : Adjustment
    {
        private readonly HighlightsAndShadowsEffect effect;

        public ShadowsHighlightsAdjustment()
        {
            this.effect = new HighlightsAndShadowsEffect();
        }

        public override void Dispose()
        {
            effect.Dispose();
        }

        public float Shadows
        {
            get
            {
                return effect.Shadows;
            }

            set
            {
                effect.Shadows = value;
                RaisePropertyChanged();
            }
        }

        public float Highlights
        {
            get
            {
                return effect.Highlights;
            }

            set
            {
                effect.Highlights = value;
                RaisePropertyChanged();
            }
        }

        public float Clarity
        {
            get
            {
                return effect.Clarity;
            }

            set
            {
                effect.Clarity = value;
                RaisePropertyChanged();
            }
        }

        internal override ICanvasImage Output => effect;

        protected override void OnInputChanged()
        {
            effect.Source = Input;
        }
    }
}