using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix.Editor
{
    public sealed partial class ImageEditorPage : Page
    {
        private StorageFile file;
        private Image image;

        public ImageEditorPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.file = (StorageFile)e.Parameter;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            image?.Dispose();
        }

        private void Canvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            if (args.Reason == CanvasCreateResourcesReason.FirstTime)
            {
                args.TrackAsyncAction(LoadImageAsync().AsAsyncAction());
            }
        }

        private async Task LoadImageAsync()
        {
            this.image = await Image.LoadAsync(file);
            canvas.Width = image.Width;
            canvas.Height = image.Height;
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            image.Draw(args.DrawingSession);
        }

        private void BlackAndWhite_Click(object sender, RoutedEventArgs e)
        {
            if (image != null)
            {
                image.ApplyBlackAndWhiteAdjustment();
                canvas.Invalidate();
            }
        }
    }
}
