using Windows.Storage.Pickers;

namespace Fotografix.UI.FileManagement
{
    public static class FilePickerFactory
    {
        public static FileOpenPicker CreateFilePicker()
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
