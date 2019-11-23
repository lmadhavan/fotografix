using System.ComponentModel;
using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal sealed class ShadowsHighlightsAdjustmentNode : AdjustmentNode
    {
        private readonly ShadowsHighlightsAdjustment adjustment;
        private readonly HighlightsAndShadowsEffect effect;

        public ShadowsHighlightsAdjustmentNode(ShadowsHighlightsAdjustment adjustment) : base(adjustment)
        {
            this.adjustment = adjustment;
            this.effect = new HighlightsAndShadowsEffect()
            {
                Shadows = adjustment.Shadows,
                Highlights = adjustment.Highlights,
                Clarity = adjustment.Clarity
            };
            this.Output = effect;
        }

        public override void Dispose()
        {
            effect.Dispose();
            base.Dispose();
        }

        protected override void OnInputChanged()
        {
            effect.Source = Input;
            Invalidate();
        }

        protected override void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ShadowsHighlightsAdjustment.Shadows):
                    effect.Shadows = adjustment.Shadows;
                    break;

                case nameof(ShadowsHighlightsAdjustment.Highlights):
                    effect.Highlights = adjustment.Highlights;
                    break;

                case nameof(ShadowsHighlightsAdjustment.Clarity):
                    effect.Clarity = adjustment.Clarity;
                    break;
            }

            base.OnAdjustmentPropertyChanged(sender, e);
        }
    }
}