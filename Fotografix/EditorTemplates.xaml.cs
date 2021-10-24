using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            Button button = (Button)sender;

            PhotoEditor editor = (PhotoEditor)button.DataContext;
            editor.ResetAdjustment();

            button.CloseParentFlyout();
        }
    }
}
