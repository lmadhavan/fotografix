using Fotografix.Editor;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
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
        private ToolAdapter toolAdapter;
        private ImageEditor editor;

        public ImageEditorPage()
        {
            this.InitializeComponent();
            BindNewAdjustmentMenuFlyout();
        }

        public void Dispose()
        {
            canvas.RemoveFromVisualTree();
        }

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
            Initialize((ImageEditor)e.Parameter);
        }

        private void Initialize(ImageEditor editor)
        {
            this.editor = editor;
            editor.Invalidated += (s, e) => canvas.Invalidate();

            this.toolAdapter = new ToolAdapter(canvas, editor.Viewport);
            toolAdapter.Toolbox = editor;

            Bindings.Update();

            /*
             * This MUST come after Bindings.Update()
             * 
             * There is an issue with ListView where the selection does not update
             * if SelectedItem is set before a CollectionChanged event is received.
             * To work around this, we need to reset the selection AFTER the ListView
             * has processed CollectionChanged, which is why our event subscription
             * must come after Bindings.Update()
             */
            editor.Layers.CollectionChanged += (s, e) => ResetSelectedLayer();
        }

        private void ResetSelectedLayer()
        {
            layerListView.SelectedItem = null;
            layerListView.SelectedItem = editor.ActiveLayer;
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            editor.Draw(args.DrawingSession);
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            editor.Viewport.Size = new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight);
        }

        private string FormatSize(Size size)
        {
            return $"{size.Width}×{size.Height}";
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
