using Fotografix.Editor;
using Windows.Globalization.NumberFormatting;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI
{
    public sealed partial class ResizeImageDialog : ContentDialog
    {
        private readonly ResizeImageParameters parameters;
        private readonly INumberFormatter2 numberFormatter;

        public ResizeImageDialog(ResizeImageParameters parameters)
        {
            this.parameters = parameters;
            this.numberFormatter = NumberFormatters.Dimension;
            this.InitializeComponent();
        }
    }
}
