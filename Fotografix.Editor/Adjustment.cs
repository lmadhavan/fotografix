using Microsoft.Graphics.Canvas;
using System;
using Windows.Graphics.Effects;

namespace Fotografix.Editor
{
    public abstract class Adjustment : IDisposable
    {
        public abstract void Dispose();

        internal abstract IGraphicsEffectSource Input { get; set; }
        internal abstract ICanvasImage Output { get; }
    }
}