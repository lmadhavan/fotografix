using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Export
{
    public sealed partial class ExportDialog : ContentDialog
    {
        private readonly ExportViewModel vm;

        public ExportDialog(ExportViewModel vm)
        {
            this.vm = vm;
            this.InitializeComponent();
        }

        private async void DestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.SettingsIdentifier = "Export";
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                vm.DestinationFolder = folder;
            }
        }
    }
}
