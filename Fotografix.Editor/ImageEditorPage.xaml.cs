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
        private ImageEditorViewModel viewModel;

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
            viewModel?.Dispose();
        }

        private void Canvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            if (args.Reason == CanvasCreateResourcesReason.FirstTime)
            {
                args.TrackAsyncAction(LoadImageAsync().AsAsyncAction());
            }
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            viewModel.Draw(args.DrawingSession);
        }

        private async Task LoadImageAsync()
        {
            this.viewModel = new ImageEditorViewModel(await Image.LoadAsync(file));
            viewModel.Invalidated += ViewModel_Invalidated;
            Bindings.Update();

            canvas.Width = viewModel.Width;
            canvas.Height = viewModel.Height;
        }

        private void ViewModel_Invalidated(object sender, EventArgs e)
        {
            canvas.Invalidate();
        }
    }
}
