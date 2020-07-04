using System.Drawing;

namespace Fotografix.Adjustments
{
    public sealed class GradientMapAdjustment : NotifyPropertyChangedBase, IGradientMapAdjustment
    {
        private Color shadows = Color.Black;
        private Color highlights = Color.White;

        public Color Shadows
        {
            get => shadows;
            set => SetProperty(ref shadows, value);
        }

        public Color Highlights
        {
            get => highlights;
            set => SetProperty(ref highlights, value);
        }
    }
}
