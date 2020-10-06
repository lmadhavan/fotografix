using Fotografix.IO;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage.Pickers;

namespace Fotografix.UI.FileManagement
{
    public static class FilePickerFactory
    {
        public static FileOpenPicker CreateFileOpenPicker(IEnumerable<FileFormat> fileFormats)
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };

            foreach (string fileExtension in fileFormats.SelectMany(f => f.FileExtensions))
            {
                picker.FileTypeFilter.Add(fileExtension);
            }

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
