using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;

namespace Fotografix
{
    internal static class TestData
    {
        internal static async Task<StorageFolder> GetFolderAsync(string name)
        {
            var folder = await GetTestDataFolderAsync();
            return await folder.GetFolderAsync(name);
        }

        internal static async Task<StorageFile> GetFileAsync(string name)
        {
            var folder = await GetTestDataFolderAsync();
            var path = Path.Combine(folder.Path, name);
            return await StorageFile.GetFileFromPathAsync(path);
        }

        internal static async Task<StorageFileReference> GetFileReferenceAsync(string name)
        {
            var folder = await GetTestDataFolderAsync();
            return new StorageFileReference(folder, name);
        }

        private static IAsyncOperation<StorageFolder> GetTestDataFolderAsync()
        {
            return Package.Current.InstalledLocation.GetFolderAsync("TestData");
        }
    }
}
