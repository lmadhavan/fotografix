using Fotografix.Drawing;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Fotografix
{
    public sealed class Image : NotifyContentChangedBase, IDrawable
    {
        private Size size;

        public Image(Size size)
        {
            this.size = size;
            this.Layers = new ObservableCollection<Layer>();
        }

        public Image(BitmapLayer layer) : this(layer.Bitmap.Size)
        {
            Layers.Add(layer);
        }

        public Size Size
        {
            get
            {
                return size;
            }

            set
            {
                SetProperty(ref size, value);
            }
        }

        public ObservableCollection<Layer> Layers { get; }

        public void Draw(IDrawingContext drawingContext)
        {
            drawingContext.Draw(this);
        }

        public Bitmap ToBitmap(IDrawingContextFactory drawingContextFactory)
        {
            Bitmap bitmap = new Bitmap(Size);
            using (IDrawingContext dc = drawingContextFactory.CreateDrawingContext(bitmap))
            {
                dc.Draw(this);
            }
            return bitmap;
        }
    }
}
