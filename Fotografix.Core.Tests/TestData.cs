using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Fotografix
{
    internal static class TestData
    {
        internal static async Task<StorageFolder> GetFolderAsync(string name)
        {
            var testData = await Package.Current.InstalledLocation.GetFolderAsync("TestData");
            return await testData.GetFolderAsync(name);
        }

        internal static async Task<StorageFile> GetFileAsync(string name)
        {
            var testData = await Package.Current.InstalledLocation.GetFolderAsync("TestData");
            return await testData.GetFileAsync(name);
        }
    }
}
