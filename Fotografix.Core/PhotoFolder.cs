using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.FileProperties;
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
            var fileInformationFactory = new FileInformationFactory(query, ThumbnailMode.SingleItem);

            var files = await fileInformationFactory.GetFilesAsync();
            return files.Select(file => new Photo(file)).ToList();
        }
    }
}
