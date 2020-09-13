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
            this.Layers = new LayerList(this);
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
                if (SetProperty(ref size, value))
                {
                    RaiseContentChanged();
                }
            }
        }

        public ObservableCollection<Layer> Layers { get; }

        public void Draw(IDrawingContext drawingContext)
        {
            drawingContext.Draw(this);
        }

        public Bitmap ToBitmap(IBitmapFactory bitmapFactory, IDrawingContextFactory drawingContextFactory)
        {
            Bitmap bitmap = bitmapFactory.CreateBitmap(size);
            using (IDrawingContext dc = drawingContextFactory.CreateDrawingContext(bitmap))
            {
                dc.Draw(this);
            }
            return bitmap;
        }

        private void OnLayerContentChanged(object sender, ContentChangedEventArgs e)
        {
            RaiseContentChanged();
        }

        private sealed class LayerList : ObservableCollection<Layer>
        {
            private readonly Image image;

            public LayerList(Image image)
            {
                this.image = image;
            }

            protected override void InsertItem(int index, Layer item)
            {
                base.InsertItem(index, item);
                item.ContentChanged += image.OnLayerContentChanged;
                image.RaiseContentChanged();
            }

            protected override void RemoveItem(int index)
            {
                this[index].ContentChanged -= image.OnLayerContentChanged;
                base.RemoveItem(index);
                image.RaiseContentChanged();
            }

            protected override void MoveItem(int oldIndex, int newIndex)
            {
                base.MoveItem(oldIndex, newIndex);
                image.RaiseContentChanged();
            }

            protected override void ClearItems()
            {
                foreach (Layer layer in this)
                {
                    layer.ContentChanged -= image.OnLayerContentChanged;
                }

                base.ClearItems();
                image.RaiseContentChanged();
            }
        }
    }
}
