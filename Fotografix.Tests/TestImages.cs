﻿using Fotografix.UI;
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

        internal static async Task<Bitmap> LoadBitmapAsync(string filename)
        {
            var file = await GetFileAsync(filename);

            using (var stream = await file.OpenReadAsync())
            {
                return await WindowsImageDecoder.ReadBitmapAsync(stream);
            }
        }
    }
}
