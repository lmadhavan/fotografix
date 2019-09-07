using Windows.UI.Xaml.Controls;

namespace Fotografix
{
    // A dummy page is required in the main project to prevent an AccessViolationException.
    public sealed partial class DummyPage : Page
    {
        public DummyPage()
        {
            this.InitializeComponent();
        }
    }
}
