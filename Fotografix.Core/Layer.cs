using System;

namespace Fotografix
{
    public abstract class Layer : NotifyPropertyChangedBase
    {
        private string name = "";
        private bool visible = true;
        private BlendMode blendMode = BlendMode.Normal;
        private float opacity = 1;

        internal Layer()
        {
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                SetProperty(ref name, value);
            }
        }

        public bool Visible
        {
            get => visible;
            set => SetProperty(ref visible, value);
        }

        public BlendMode BlendMode
        {
            get => blendMode;
            set => SetProperty(ref blendMode, value);
        }

        public float Opacity
        {
            get
            {
                return opacity;
            }

            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref opacity, value);
            }
        }

        public virtual bool CanPaint => false;

        public abstract void Accept(LayerVisitor visitor);

        public virtual void Paint(BrushStroke brushStroke)
        {
            throw new NotSupportedException();
        }
    }
}