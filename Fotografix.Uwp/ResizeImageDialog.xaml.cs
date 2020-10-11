using Fotografix.Editor;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp
{
    public sealed partial class ResizeImageDialog : ContentDialog
    {
        private readonly ResizeImageParameters parameters;

        public ResizeImageDialog(ResizeImageParameters parameters)
        {
            this.parameters = parameters;
            this.InitializeComponent();
        }
    }
}
