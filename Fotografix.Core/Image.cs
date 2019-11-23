using System.Collections.ObjectModel;
using System.Drawing;

namespace Fotografix
{
    public sealed class Image : NotifyPropertyChangedBase
    {
        private Size size;

        public Image(Size size)
        {
            this.size = size;
            this.Layers = new ObservableCollection<Layer>();
        }

        public Size Size
        {
            get => size;
            set => SetProperty(ref size, value);
        }

        public ObservableCollection<Layer> Layers { get; }
    }
}
