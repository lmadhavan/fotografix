using Fotografix.Uwp.Codecs;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
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
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await WindowsImageEncoder.WriteBitmapAsync(BitmapEncoder.PngEncoderId, stream, bitmap);
            }

            FolderLauncherOptions options = new FolderLauncherOptions();
            options.ItemsToSelect.Add(file);
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await Launcher.LaunchFolderAsync(folder));
        }
    }
}
