using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace Fotografix
{
    public sealed partial class EditorTemplates : ResourceDictionary
    {
        public EditorTemplates()
        {
            InitializeComponent();
        }

        private async void EditorControls_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled)
            {
                // don't pop up dialog in XAML designer
                return;
            }

            await WelcomeDialog.ShowOnFirstLoadAsync();
        }
    }
}
