using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Fotografix
{
    public sealed partial class WelcomeDialog : ContentDialog
    {
        public WelcomeDialog()
        {
            this.InitializeComponent();
        }

        public static async Task ShowOnFirstLoadAsync()
        {
            const string settingKey = "onboardingComplete";
            var settings = ApplicationData.Current.LocalSettings.Values;

            if (settings.ContainsKey(settingKey))
            {
                return;
            }

            await new WelcomeDialog().ShowAsync();
            settings[settingKey] = true;
        }

        private int NumberOfPages => 4;

        private void Next_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (flipView.SelectedIndex < NumberOfPages - 1)
            {
                flipView.SelectedIndex++;
                args.Cancel = true;
            }
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flipView.SelectedIndex == NumberOfPages - 1)
            {
                PrimaryButtonText = "";
                SecondaryButtonText = "Let's go!";
            }
        }
    }
}
