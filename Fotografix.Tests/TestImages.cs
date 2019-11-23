using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Fotografix.Tests
{
    internal static class TestImages
    {
        internal static async Task<StorageFile> GetFileAsync(string filename)
        {
            var imagesFolder = await Package.Current.InstalledLocation.GetFolderAsync("Images");
            return await imagesFolder.GetFileAsync(filename);
        }
    }
}
