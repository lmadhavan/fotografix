using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Editor.Adjustments
{
    public sealed class ShadowsHighlightsAdjustment : Adjustment
    {
        private readonly HighlightsAndShadowsEffect effect;

        public ShadowsHighlightsAdjustment()
        {
            this.Name = "Shadows/Highlights";
            this.effect = new HighlightsAndShadowsEffect();
            this.RawOutput = effect;
        }

        public override void Dispose()
        {
            base.Dispose();
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

        protected override void OnInputChanged()
        {
            effect.Source = Input;
        }
    }
}