using System;

namespace Fotografix
{
    public abstract class Layer : NotifyContentChangedBase
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
            get
            {
                return visible;
            }

            set
            {
                if (SetProperty(ref visible, value))
                {
                    RaiseContentChanged();
                }
            }
        }

        public BlendMode BlendMode
        {
            get
            {
                return blendMode;
            }

            set
            {
                if (SetProperty(ref blendMode, value))
                {
                    RaiseContentChanged();
                }
            }
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

                if (SetProperty(ref opacity, value))
                {
                    RaiseContentChanged();
                }
            }
        }

        public virtual bool CanPaint => false;

        public virtual IUndoable Paint(BrushStroke brushStroke)
        {
            throw new NotSupportedException();
        }
    }
}