using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix
{
    public sealed partial class MainPage : Page
    {
        private ApplicationViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.vm = (ApplicationViewModel)e.Parameter;
            vm.EditorInvalidated += (s, e2) => canvas.Invalidate();
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
