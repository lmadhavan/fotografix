using Microsoft.Graphics.Canvas;
using System;
using Windows.Graphics.Effects;

namespace Fotografix.Editor.Adjustments
{
    public abstract class Adjustment : NotifyPropertyChangedBase, IDisposable
    {
        private string name;

        public abstract void Dispose();

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                SetValue(ref name, value);
            }
        }

        internal abstract IGraphicsEffectSource Input { get; set; }
        internal abstract ICanvasImage Output { get; }
    }
}