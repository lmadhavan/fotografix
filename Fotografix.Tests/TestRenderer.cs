using Fotografix.Win2D;
using Microsoft.Graphics.Canvas;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;

namespace Fotografix.Tests
{
    public static class TestRenderer
    {
        public static async Task RenderToTempFolderAsync(IWin2DDrawable drawable, string filename)
        {
            using (var renderTarget = Render(drawable))
            {
                await SaveToTempFolderAsync(renderTarget, filename);
            }
        }

        private static async Task SaveToTempFolderAsync(CanvasRenderTarget renderTarget, string filename)
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

        public static CanvasRenderTarget Render(IWin2DDrawable drawable)
        {
            CanvasRenderTarget renderTarget = CreateRenderTarget(drawable.Size);

            using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
            {
                drawable.Draw(ds);
            }

            return renderTarget;
        }

        private static CanvasRenderTarget CreateRenderTarget(Size size)
        {
            return new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), size.Width, size.Height, 96);
        }
    }
}
