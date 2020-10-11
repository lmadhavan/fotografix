using System;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp
{
    public sealed partial class ErrorDialog : ContentDialog
    {
        private readonly Exception exception;

        public ErrorDialog(Exception ex)
        {
            this.InitializeComponent();
            this.exception = ex;
        }

        private string Error => exception.Message + Environment.NewLine + exception.StackTrace;
    }
}
