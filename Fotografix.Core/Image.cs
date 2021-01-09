using Fotografix.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Fotografix
{
    public sealed class Image : ImageElement
    {
        private readonly LayerList layers;
        private Size size;

        public Image(Size size)
        {
            this.size = size;
            this.layers = new LayerList(this);
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

        public ObservableCollection<Layer> Layers => layers;

        public IList<Layer> DetachLayers()
        {
            return layers.Detach();
        }

        private sealed class LayerList : ObservableCollection<Layer>
        {
            private readonly Image image;

            internal LayerList(Image image)
            {
                this.image = image;
            }

            protected override void InsertItem(int index, Layer item)
            {
                image.AddChild(item);
                base.InsertItem(index, item);
                image.RaiseContentChanged(new AddItemChange<Layer>(this, index, item));
            }

            protected override void RemoveItem(int index)
            {
                Layer item = this[index];
                image.RemoveChild(item);
                base.RemoveItem(index);
                image.RaiseContentChanged(new RemoveItemChange<Layer>(this, index, item));
            }

            protected override void SetItem(int index, Layer newItem)
            {
                Layer oldItem = this[index];
                image.RemoveChild(oldItem);
                image.AddChild(newItem);
                base.SetItem(index, newItem);
                image.RaiseContentChanged(new ReplaceItemChange<Layer>(this, index, oldItem, newItem));
            }

            protected override void MoveItem(int oldIndex, int newIndex)
            {
                base.MoveItem(oldIndex, newIndex);
                image.RaiseContentChanged(new MoveItemChange<Layer>(this, oldIndex, newIndex));
            }

            protected override void ClearItems()
            {
                throw new NotSupportedException();
            }

            internal IList<Layer> Detach()
            {
                foreach (Layer layer in this)
                {
                    image.RemoveChild(layer);
                }

                List<Layer> copy = new List<Layer>(this);
                base.ClearItems();
                return copy;
            }
        }
    }
}
