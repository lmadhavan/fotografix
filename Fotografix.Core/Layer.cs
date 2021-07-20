using Fotografix.Adjustments;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Fotografix
{
    public sealed class Layer : ImageElement
    {
        private readonly Channel contentChannel;

        private string name = "";
        private bool visible = true;
        private BlendMode blendMode = BlendMode.Normal;
        private float opacity = 1;

        public Layer() : this(new Bitmap(Size.Empty))
        {
        }

        public Layer(Bitmap bitmap) : this(new BitmapChannel(bitmap))
        {
        }

        public Layer(Adjustment adjustment) : this(new AdjustmentChannel(adjustment))
        {
        }

        public Layer(Channel contentChannel)
        {
            this.contentChannel = contentChannel;
            contentChannel.PropertyChanged += ContentChannel_PropertyChanged;
            AddChild(contentChannel);
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

        public Channel ContentChannel => contentChannel;
        public ImageElement Content => contentChannel.Content;

        public override string ToString()
        {
            return $"Layer[{name}]";
        }

        internal void Crop(Rectangle rectangle)
        {
            contentChannel.Crop(rectangle);
        }

        internal void Scale(PointF scaleFactor, IGraphicsDevice graphicsDevice)
        {
            contentChannel.Scale(scaleFactor, graphicsDevice);
        }

        private void ContentChannel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Channel.Content))
            {
                RaisePropertyChanged(nameof(Content));
            }
        }
    }
}