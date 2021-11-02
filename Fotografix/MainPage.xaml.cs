using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix
{
    public sealed partial class MainPage : Page
    {
        private ApplicationViewModel vm;
        private EditorViewModel editor;

        public MainPage()
        {
            this.InitializeComponent();
            CustomTitleBar.Initialize(appTitleBar, appTitle);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.vm = (ApplicationViewModel)e.Parameter;
            vm.CanvasResourceCreator = canvas;
            vm.EditorLoaded += OnEditorLoaded;
        }

        private void OnEditorLoaded(object sender, EditorViewModel e)
        {
            this.editor = e;
            editor.Invalidated += (s, _) => canvas.Invalidate();
            editor.SetViewportSize(new Size(viewport.ActualWidth, viewport.ActualHeight));
            canvas.Invalidate();
        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            await vm.DisposeAsync();
        }


        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            editor?.Draw(args.DrawingSession);
        }

        private void Viewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            editor?.SetViewportSize(e.NewSize);
        }

        private void RecentFolderFlyout_FolderActivated(object sender, StorageFolder folder)
        {
            vm.OpenFolder(folder);
        }

        private async void PickFolder()
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                recentFolderFlyout.Add(folder);
                vm.OpenFolder(folder);
            }
        }

        private string FormatPhotoCount(int count)
        {
            string suffix = count == 1 ? "" : "s";
            return $"({count} photo{suffix})";
        }
    }
}
