﻿using Fotografix.UI.Adjustments;
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
    public sealed partial class ImageEditorPage : Page
    {
        private StorageFile file;
        private ImageEditor editor;
        private ResizeImageParameters resizeImageParameters;

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
            editor.AddAdjustmentLayer(adjustmentLayerFactory);
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

            this.editor = await ImageEditor.CreateAsync(file);
            editor.Invalidated += OnEditorInvalidated;
            editor.PropertyChanged += OnEditorPropertyChanged;

            Bindings.Update();
            UpdateCanvasSize();
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