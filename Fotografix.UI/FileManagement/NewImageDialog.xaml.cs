using Windows.Globalization.NumberFormatting;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI.FileManagement
{
    public sealed partial class NewImageDialog : ContentDialog
    {
        private readonly NewImageParameters parameters;
        private readonly INumberFormatter2 numberFormatter;

        public NewImageDialog(NewImageParameters parameters)
        {
            this.parameters = parameters;
            this.numberFormatter = NumberFormatters.Dimension;
            this.InitializeComponent();
        }
    }
}
