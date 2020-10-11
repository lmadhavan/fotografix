using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp
{
    public sealed partial class ColorPickerButton : UserControl
    {
        public ColorPickerButton()
        {
            InitializeComponent();
        }

        public Color Color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }

            set
            {
                if (Color != value)
                {
                    SetValue(ColorProperty, value);
                }
            }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(int), typeof(ColorPickerButton), new PropertyMetadata(Colors.Black));
    }
}
