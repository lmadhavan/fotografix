using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix
{
    public sealed partial class MainPage : Page
    {
        private readonly ApplicationViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();
            this.vm = new ApplicationViewModel();
            vm.EditorInvalidated += (s, e) => canvas.Invalidate();
        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            await vm.DisposeAsync();
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var editor = vm.Editor?.Result;

            if (editor != null)
            {
                canvas.Width = editor.Size.Width;
                canvas.Height = editor.Size.Height;
                editor.Draw(args.DrawingSession);
            }
        }
    }
}
