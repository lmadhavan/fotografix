using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Fotografix
{
    public static class PhotoFolder
    {
        public static async Task<IReadOnlyList<Photo>> GetPhotosAsync(StorageFolder folder)
        {
            var queryOptions = new QueryOptions();
            queryOptions.FileTypeFilter.Add(".jpg");

            var query = folder.CreateFileQueryWithOptions(queryOptions);
            var files = await query.GetFilesAsync();
            return files.Select(file => new Photo(file)).ToList();
        }
    }
}
