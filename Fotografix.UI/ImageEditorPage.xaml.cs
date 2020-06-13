using Fotografix.Editor;
using Fotografix.Editor.Tools;
using Fotografix.UI.Adjustments;
using Fotografix.UI.FileManagement;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Fotografix.UI
{
    public sealed partial class ImageEditorPage : Page, IDisposable
    {
        private static readonly Dictionary<ToolCursor, CoreCursor> CursorMap = new Dictionary<ToolCursor, CoreCursor>
        {
            [ToolCursor.Crosshair] = new CoreCursor(CoreCursorType.Cross, 0),
            [ToolCursor.Disabled] = new CoreCursor(CoreCursorType.UniversalNo, 0)
        };

        private readonly CoreWindow window;
        private readonly Viewport viewport;

        private CoreCursor originalCursor;
        private IWorkspace workspace;
        private ICreateImageEditorCommand createCommand;
        private ImageEditor editor;
        private ResizeImageParameters resizeImageParameters;

        public ImageEditorPage()
        {
            this.window = CoreWindow.GetForCurrentThread();
            this.InitializeComponent();
            this.viewport = new ScrollViewerViewport(scrollViewer);
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

            var parameters = (ImageEditorPageParameters)e.Parameter;
            this.workspace = parameters.Workspace;
            this.createCommand = parameters.Command;
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

            this.editor = await createCommand.ExecuteAsync();
            editor.Invalidated += OnEditorInvalidated;
            editor.PropertyChanged += OnEditorPropertyChanged;

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

        private async void NewImage_Click(object sender, RoutedEventArgs e)
        {
            await workspace.NewImageAsync();
        }

        private async void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            await workspace.OpenFileAsync();
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = FilePickerFactory.CreateFilePicker();
            picker.CommitButtonText = "Import";

            var files = await picker.PickMultipleFilesAsync();
            await editor.ImportLayersAsync(files);
        }

        private void ResizeImageFlyout_Opened(object sender, object e)
        {
            this.resizeImageParameters = editor.CreateResizeImageParameters();
            this.resizeImageFlyoutContent.Content = resizeImageParameters;
        }

        private void ResizeImage_Click(object sender, RoutedEventArgs e)
        {
            resizeImageFlyout.Hide();
            editor.ResizeImage(resizeImageParameters);
        }

        private void Canvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.originalCursor = window.PointerCursor;
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            canvas.CapturePointer(e.Pointer);
            editor?.PointerPressed(GetEditorPoint(e));
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            window.PointerCursor = CursorMap[editor?.ToolCursor ?? ToolCursor.Disabled];
            editor?.PointerMoved(GetEditorPoint(e));
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            editor?.PointerReleased(GetEditorPoint(e));
            canvas.ReleasePointerCapture(e.Pointer);
        }

        private void Canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            window.PointerCursor = originalCursor;
        }

        private PointF GetEditorPoint(PointerRoutedEventArgs e)
        {
            PointerPoint pt = e.GetCurrentPoint(canvas);
            return new PointF((float)pt.Position.X, (float)pt.Position.Y);
        }
    }
}
