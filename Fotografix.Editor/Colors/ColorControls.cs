using System.Drawing;

namespace Fotografix.Editor.Colors
{
    public sealed class ColorControls : NotifyPropertyChangedBase
    {
        public static readonly Color DefaultForegroundColor = Color.Black;
        public static readonly Color DefaultBackgroundColor = Color.White;

        private bool foregroundActive = true;
        private Color foregroundColor = DefaultForegroundColor;
        private Color backgroundColor = DefaultBackgroundColor;

        public bool IsForegroundColorActive
        {
            get => foregroundActive;

            set
            {
                if (SetProperty(ref foregroundActive, value))
                {
                    RaisePropertyChanged(nameof(IsBackgroundColorActive));
                    RaisePropertyChanged(nameof(ActiveColor));
                }
            }
        }

        public bool IsBackgroundColorActive
        {
            get => !foregroundActive;

            set
            {
                if (SetProperty(ref foregroundActive, !value))
                {
                    RaisePropertyChanged(nameof(IsForegroundColorActive));
                    RaisePropertyChanged(nameof(ActiveColor));
                }
            }
        }

        public Color ActiveColor
        {
            get => foregroundActive ? ForegroundColor : BackgroundColor;

            set
            {
                if (foregroundActive)
                {
                    this.ForegroundColor = value;
                }
                else
                {
                    this.BackgroundColor = value;
                }
            }
        }

        public Color ForegroundColor
        {
            get => foregroundColor;
            
            set
            {
                if (SetProperty(ref foregroundColor, value) && IsForegroundColorActive)
                {
                    RaisePropertyChanged(nameof(ActiveColor));
                }
            }
        }

        public Color BackgroundColor
        {
            get => backgroundColor;

            set
            {
                if (SetProperty(ref backgroundColor, value) && IsBackgroundColorActive)
                {
                    RaisePropertyChanged(nameof(ActiveColor));
                }
            }
        }
    }
}
