using Fotografix.Editor;
using Fotografix.Uwp.FileManagement;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix.Uwp
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
            this.viewport = new Viewport();
            this.toolAdapter = new ToolAdapter(canvas, viewport);
            BindNewAdjustmentMenuFlyout();
        }

        public void Dispose()
        {
            canvas.RemoveFromVisualTree();
            editor?.Dispose();
        }

        public string Title => editor?.Title ?? "Loading...";
        public event EventHandler TitleChanged;

        private void BindNewAdjustmentMenuFlyout()
        {
            var menuFlyout = (MenuFlyout)newAdjustmentButton.Flyout;
            foreach (MenuFlyoutItem item in menuFlyout.Items)
            {
                item.Click += NewAdjustmentMenuItem_Click;
            }
        }

        private async void NewAdjustmentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuFlyoutItem)sender;
            var command = (IDocumentCommand)item.Tag;
            await editor.ExecuteAsync(command);
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

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            viewport.Size = new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight);
        }

        private async Task LoadImageAsync()
        {
            this.editor = await createCommand.ExecuteAsync(viewport);
            editor.Invalidated += Editor_Invalidated;
            editor.PropertyChanged += Editor_PropertyChanged;

            toolAdapter.Toolbox = editor;

            Bindings.Update();
            TitleChanged?.Invoke(this, EventArgs.Empty);
        }

        private string FormatSize(Size size)
        {
            return $"{size.Width}×{size.Height}";
        }

        private void Editor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageEditor.Title))
            {
                TitleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Editor_Invalidated(object sender, EventArgs e)
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
                await ImportLayersAsync(items.OfType<StorageFile>());
            }
        }

        private Task ImportLayersAsync(IEnumerable<StorageFile> files)
        {
            return editor.ImportLayersAsync(files.Select(f => new StorageFileAdapter(f)));
        }
    }
}
