using System.Drawing;

namespace Fotografix.Adjustments
{
    public abstract class GradientMapAdjustment : Adjustment, IGradientMapAdjustment
    {
        private Color shadows = Color.Black;
        private Color highlights = Color.White;

        public Color Shadows
        {
            get => shadows;

            set
            {
                if (SetProperty(ref shadows, value))
                {
                    OnShadowsChanged();
                }
            }
        }

        public Color Highlights
        {
            get => highlights;

            set
            {
                if (SetProperty(ref highlights, value))
                {
                    OnHighlightsChanged();
                }
            }
        }

        protected virtual void OnShadowsChanged()
        {
        }

        protected virtual void OnHighlightsChanged()
        {
        }
    }
}
