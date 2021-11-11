using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Fotografix
{
    public sealed partial class MainPage : Page
    {
        private ApplicationViewModel vm;
        private EditorViewModel editor;
        private PanOperation panOperation;

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

        private void Canvas_RegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            foreach (var rect in args.InvalidatedRegions)
            {
                using (var ds = sender.CreateDrawingSession(rect))
                {
                    editor?.Draw(ds);
                }
            }
        }

        private void Viewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            editor?.SetViewportSize(e.NewSize);
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && canvas.CapturePointer(e.Pointer))
            {
                this.panOperation = new PanOperation(viewport, e);
            }
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            panOperation?.Track(e);
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (panOperation != null)
            {
                canvas.ReleasePointerCapture(e.Pointer);
                this.panOperation = null;
            }
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
