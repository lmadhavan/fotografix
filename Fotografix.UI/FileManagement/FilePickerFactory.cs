using System.Collections.Generic;
using Windows.Storage.Pickers;

namespace Fotografix.UI.FileManagement
{
    public static class FilePickerFactory
    {
        public static FileOpenPicker CreateFileOpenPicker()
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

        public static FileSavePicker CreateFileSavePicker()
        {
            FileSavePicker picker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeChoices.Add("JPEG", new List<string> { ".jpg" });
            picker.FileTypeChoices.Add("PNG", new List<string> { ".png" });
            return picker;
        }
    }
}
