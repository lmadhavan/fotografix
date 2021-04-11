using System;
using System.Drawing;

namespace Fotografix
{
    public sealed class Layer : ImageElement
    {
        private string name = "";
        private bool visible = true;
        private BlendMode blendMode = BlendMode.Normal;
        private float opacity = 1;
        private ContentElement content;

        public Layer() : this(new Bitmap(Size.Empty))
        {
        }

        public Layer(ContentElement content)
        {
            this.Content = content;
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
                SetProperty(ref visible, value);
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
                SetProperty(ref blendMode, value);
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

                SetProperty(ref opacity, value);
            }
        }

        public ContentElement Content
        {
            get
            {
                return content;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                SetChild(ref content, value);
            }
        }
    }
}