using Fotografix.Editor;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI
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
