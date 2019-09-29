using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
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
            BindNewAdjustmentMenuFlyout();
        }

        private void BindNewAdjustmentMenuFlyout()
        {
            var menuFlyout = (MenuFlyout)newAdjustmentButton.Flyout;
            foreach (MenuFlyoutItem item in menuFlyout.Items)
            {
                item.Click += NewAdjustmentMenuItem_Click;
            }
        }

        private void NewAdjustmentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuFlyoutItem)sender;
            var adjustmentFactory = (IAdjustmentFactory)item.Tag;
            viewModel?.AddAdjustment(adjustmentFactory.CreateAdjustment());
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
            viewModel?.Dispose();

            this.viewModel = new ImageEditorViewModel(await Image.LoadAsync(file));
            viewModel.Invalidated += ViewModel_Invalidated;

            Bindings.Update();

            canvas.Width = viewModel.Width;
            canvas.Height = viewModel.Height;
            canvas.Invalidate();
        }

        private void ViewModel_Invalidated(object sender, EventArgs e)
        {
            canvas.Invalidate();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Link;
                e.DragUIOverride.Caption = "Open";
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        protected async override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                
                if (items[0] is StorageFile file)
                {
                    this.file = file;
                    await LoadImageAsync();
                }
            }
        }

        private async void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            this.file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await LoadImageAsync();
            }
        }
    }
}
