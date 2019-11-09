using Fotografix.Composition;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix.UI
{
    public sealed partial class ImageEditorPage : Page
    {
        private StorageFile file;
        private ImageEditor editor;

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
            var adjustmentLayerFactory = (IAdjustmentLayerFactory)item.Tag;
            editor.AddLayer(adjustmentLayerFactory.CreateAdjustmentLayer());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.file = (StorageFile)e.Parameter;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            editor?.Dispose();
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
            editor.Draw(args.DrawingSession);
        }

        private async Task LoadImageAsync()
        {
            editor?.Dispose();

            this.editor = await ImageEditor.CreateAsync(canvas, file);
            editor.Invalidated += OnEditorInvalidated;

            Bindings.Update();

            canvas.Width = editor.Width;
            canvas.Height = editor.Height;
            canvas.Invalidate();
        }

        private void OnEditorInvalidated(object sender, EventArgs e)
        {
            canvas.Invalidate();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.Caption = "Import";
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
                var files = items.OfType<StorageFile>().ToList();

                if (files.Count > 0)
                {
                    await editor.ImportAsync(files);
                }
            }
        }

        private async void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = CreateFilePicker();

            this.file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await LoadImageAsync();
            }
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = CreateFilePicker();
            picker.CommitButtonText = "Import";

            var files = await picker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                await editor.ImportAsync(files);
            }
        }

        private static FileOpenPicker CreateFilePicker()
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            return picker;
        }
    }
}
