using Fotografix.Editor.FileManagement;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp.FileManagement
{
    public sealed partial class NewImageDialog : ContentDialog
    {
        private readonly NewImageParameters parameters;

        public NewImageDialog(NewImageParameters parameters)
        {
            this.parameters = parameters;
            this.InitializeComponent();
        }
    }
}
