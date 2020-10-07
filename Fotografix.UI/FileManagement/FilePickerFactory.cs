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

        public static FileSavePicker CreateFileSavePicker(IEnumerable<FileFormat> fileFormats)
        {
            FileSavePicker picker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            foreach (FileFormat format in fileFormats)
            {
                picker.FileTypeChoices.Add(format.Name, format.FileExtensions.ToList());
            }
            
            return picker;
        }
    }
}
