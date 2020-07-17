using Fotografix.UI;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;

namespace Fotografix.Tests
{
    public static class BitmapCaptureExtensions
    {
        public static async Task CaptureToTempFolderAsync(this Bitmap bitmap, string filename)
        {
            var folder = ApplicationData.Current.TemporaryFolder;
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

            await BitmapCodec.SaveBitmapAsync(file, bitmap);

            FolderLauncherOptions options = new FolderLauncherOptions();
            options.ItemsToSelect.Add(file);
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await Launcher.LaunchFolderAsync(folder));
        }
    }
}
