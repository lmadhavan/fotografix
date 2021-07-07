using Fotografix.Editor;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp
{
    public sealed class ContentDialogAdapter<TDialog, TParameters> : IDialog<TParameters> where TDialog : ContentDialog
    {
        public async Task<bool> ShowAsync(TParameters parameters)
        {
            ContentDialog dialog = (ContentDialog)Activator.CreateInstance(typeof(TDialog), parameters);
            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
