using Fotografix.Input;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
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

        private readonly CoreWindow window;
        private readonly ViewportPanHandler panHandler;
        private IPointerEventHandler pointerEventHandler;

        public MainPage()
        {
            this.InitializeComponent();
            CustomTitleBar.Initialize(appTitleBar, appTitle);

            this.window = CoreWindow.GetForCurrentThread();
            this.panHandler = new ViewportPanHandler(viewport);
            this.pointerEventHandler = panHandler;
        }

        private AboutViewModel About { get; } = new AboutViewModel();

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
            this.pointerEventHandler = new PointerEventHandlerChain(new IPointerEventHandler[]
            {
                new PointerEventAdapter(canvas, editor),
                panHandler
            });

            editor.Invalidated += Editor_Invalidated;
            editor.SetViewportSize(new Size(viewport.ActualWidth, viewport.ActualHeight));
            canvas.Invalidate();
        }

        private void Editor_Invalidated(object sender, EventArgs e)
        {
            _ = Dispatcher.RunIdleAsync(_ => canvas.Invalidate());
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

        private void Filmstrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.SelectedPhotos = filmstrip.SelectedItems.OfType<PhotoViewModel>().ToList();
        }

        #region Pointer events

        private static readonly CoreCursor DefaultCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        private bool pointerCaptured;
        private bool pointerInside;

        private void Canvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.pointerInside = true;
        }

        private void Canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.pointerInside = false;

            if (!pointerCaptured)
            {
                window.PointerCursor = DefaultCursor;
            }
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (pointerEventHandler.PointerPressed(e))
            {
                viewport.ManipulationMode &= ~ManipulationModes.System;
                this.pointerCaptured = canvas.CapturePointer(e.Pointer);
                window.PointerCursor = pointerEventHandler.Cursor;
                e.Handled = true;
            }
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (pointerEventHandler.PointerMoved(e))
            {
                window.PointerCursor = pointerEventHandler.Cursor;
                e.Handled = true;
            }
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (pointerEventHandler.PointerReleased(e))
            {
                window.PointerCursor = pointerInside ? pointerEventHandler.Cursor : DefaultCursor;
                e.Handled = true;
            }

            canvas.ReleasePointerCapture(e.Pointer);
            viewport.ManipulationMode |= ManipulationModes.System;
            this.pointerCaptured = false;
        }

        #endregion

        private void RecentFolderFlyout_FolderActivated(object sender, StorageFolder folder)
        {
            vm.Folder = folder;
        }

        private async void PickFolder()
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                recentFolderFlyout.Add(folder);
                vm.Folder = folder;
            }
        }

        private string FormatTotalPhotoCount(int count)
        {
            return $"({PhotoViewModel.FormatPhotoCount(count)})";
        }

        private string FormatSelectedPhotoCount(int count)
        {
            return $"{PhotoViewModel.FormatPhotoCount(count)} selected";
        }

        private async void WelcomeTour_Click(object sender, RoutedEventArgs e)
        {
            await new WelcomeDialog().ShowAsync();
        }

        private async void HelpFlyout_LaunchUri(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri((string)((MenuFlyoutItem)sender).Tag);
            await Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_DragOver(object sender, DragEventArgs e)
        {
            var deferral = e.GetDeferral();

            try
            {
                e.AcceptedOperation = DataPackageOperation.None;

                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await e.DataView.GetStorageItemsAsync();

                    if (items.All(item => item is StorageFile))
                    {
                        e.AcceptedOperation = DataPackageOperation.Copy;
                    }
                }
            }
            finally
            {
                deferral.Complete();
            }
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();

                StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
                tempFolder = await tempFolder.CreateFolderAsync(Guid.NewGuid().ToString());
                tempFolder = await tempFolder.CreateFolderAsync("Dropped files");

                await Task.WhenAll(items.OfType<StorageFile>().Select(
                        f => f.CopyAsync(tempFolder, f.Name, NameCollisionOption.GenerateUniqueName).AsTask()
                ));

                vm.Folder = tempFolder;
            }
        }
    }
}
