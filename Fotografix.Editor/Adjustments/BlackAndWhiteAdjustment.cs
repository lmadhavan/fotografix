using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Editor.Adjustments
{
    public sealed class BlackAndWhiteAdjustment : Adjustment, IDisposable
    {
        private readonly GrayscaleEffect grayscaleEffect;

        public BlackAndWhiteAdjustment()
        {
            this.grayscaleEffect = new GrayscaleEffect();
            this.RawOutput = grayscaleEffect;
        }

        public override void Dispose()
        {
            base.Dispose();
            grayscaleEffect.Dispose();
        }

        protected override void OnInputChanged()
        {
            grayscaleEffect.Source = Input;
        }
    }
}