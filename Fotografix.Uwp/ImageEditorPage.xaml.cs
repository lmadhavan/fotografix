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
        }

        public void Dispose()
        {
            canvas.RemoveFromVisualTree();
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
            toolAdapter.Toolbox = editor.Toolbox;
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
            return editor.ImportLayerCommand.ExecuteAsync(files.Select(f => new StorageFileAdapter(f)));
        }
    }
}
