using Fotografix.Editor;
using Fotografix.UI.Adjustments;
using Fotografix.UI.FileManagement;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.ComponentModel;
using System.Drawing;
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
    public sealed partial class ImageEditorPage : Page, IDisposable
    {
        private readonly Viewport viewport;
        private readonly ToolAdapter toolAdapter;

        private ICreateImageEditorCommand createCommand;
        private ImageEditor editor;

        public ImageEditorPage()
        {
            this.InitializeComponent();
            this.viewport = new ScrollViewerViewport(scrollViewer);
            this.toolAdapter = new ToolAdapter(canvas, scrollViewer);
            BindNewAdjustmentMenuFlyout();
        }

        public void Dispose()
        {
            canvas.RemoveFromVisualTree();
            editor?.Dispose();
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
            editor.AddAdjustmentLayer(adjustmentLayerFactory);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.createCommand = (ICreateImageEditorCommand)e.Parameter;
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
            this.editor = await createCommand.ExecuteAsync(viewport);
            editor.Invalidated += OnEditorInvalidated;
            editor.PropertyChanged += OnEditorPropertyChanged;
            toolAdapter.Toolbox = editor;

            Bindings.Update();
            UpdateCanvasSize();
            FitToScreen();
        }

        private string FormatSize(Size size)
        {
            return $"{size.Width}×{size.Height}";
        }

        private void UpdateCanvasSize()
        {
            canvas.Width = editor.Size.Width;
            canvas.Height = editor.Size.Height;
            canvas.Invalidate();
        }

        private void FitToScreen()
        {
            viewport.ZoomToFit(editor.Size);
        }

        private void OnEditorInvalidated(object sender, EventArgs e)
        {
            canvas.Invalidate();
        }

        private void OnEditorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageEditor.Size))
            {
                UpdateCanvasSize();
            }
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
                await editor.ImportLayersAsync(items.OfType<StorageFile>());
            }
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = FilePickerFactory.CreateFilePicker();
            picker.CommitButtonText = "Import";

            var files = await picker.PickMultipleFilesAsync();
            await editor.ImportLayersAsync(files);
        }

        private async void ResizeImage_Click(object sender, RoutedEventArgs e)
        {
            ResizeImageParameters parameters = editor.CreateResizeImageParameters();

            ResizeImageDialog dialog = new ResizeImageDialog(parameters);
            if (await dialog.ShowAsync(ContentDialogPlacement.InPlace) == ContentDialogResult.Primary)
            {
                editor.ResizeImage(parameters);
            }
        }
    }
}
