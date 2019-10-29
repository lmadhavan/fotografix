using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Adjustments
{
    public sealed class BlackAndWhiteAdjustment : Adjustment, IDisposable
    {
        private readonly GrayscaleEffect grayscaleEffect;

        public BlackAndWhiteAdjustment()
        {
            this.grayscaleEffect = new GrayscaleEffect();
        }

        public override void Dispose()
        {
            grayscaleEffect.Dispose();
        }

        internal override ICanvasImage Output => grayscaleEffect;

        protected override void OnInputChanged()
        {
            base.OnInputChanged();
            grayscaleEffect.Source = Input;
        }
    }
}