using Windows.UI.Xaml;

namespace Fotografix
{
    public sealed partial class EditorTemplates : ResourceDictionary
    {
        public EditorTemplates()
        {
            InitializeComponent();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            EditorFor(sender).ResetAdjustment();
            ((FrameworkElement)sender).CloseParentFlyout();
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            await EditorFor(sender).ExportAsync();
        }

        private EditorViewModel EditorFor(object sender)
        {
            return (EditorViewModel)((FrameworkElement)sender).DataContext;
        }
    }
}
