using Microsoft.Graphics.Canvas;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;

namespace Fotografix.Editor.Tests
{
    public static class ImageCaptureExtensions
    {
        /// <summary>
        /// Captures a snapshot of the image in a temporary folder and launches File Explorer to view the contents of the folder.
        /// This is useful for generating the expected output for regression tests.
        /// </summary>
        public static async Task CaptureToTempFolderAsync(this Image image, string filename)
        {
            using (var renderTarget = image.Render())
            {
                var folder = ApplicationData.Current.TemporaryFolder;
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
                }

                FolderLauncherOptions options = new FolderLauncherOptions();
                options.ItemsToSelect.Add(file);
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await Launcher.LaunchFolderAsync(folder));
            }
        }

        public static CanvasRenderTarget Render(this Image image)
        {
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), image.Width, image.Height, 96);

            using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
            {
                image.Draw(ds);
            }

            return renderTarget;
        }
    }
}
