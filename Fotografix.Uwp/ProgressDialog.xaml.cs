using System;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp
{
    public sealed partial class ProgressDialog : ContentDialog
    {
        private readonly ProgressViewModel vm;

        public ProgressDialog(ProgressViewModel vm)
        {
            this.InitializeComponent();

            this.vm = vm;
            vm.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProgressViewModel.IsVisible))
            {
                if (vm.IsVisible)
                {
                    await ShowAsync();
                }
                else
                {
                    Hide();
                }
            }
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (vm.IsVisible)
            {
                args.Cancel = true;
            }
        }
    }
}
