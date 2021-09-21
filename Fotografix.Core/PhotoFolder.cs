using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Fotografix
{
    public sealed class PhotoFolder
    {
        private readonly StorageFolder contentFolder;
        private readonly StorageFolder sidecarFolder;

        public PhotoFolder(StorageFolder contentFolder, StorageFolder sidecarFolder)
        {
            this.contentFolder = contentFolder;
            this.sidecarFolder = sidecarFolder;
        }

        public async Task<IReadOnlyList<Photo>> GetPhotosAsync()
        {
            var queryOptions = new QueryOptions();
            queryOptions.FileTypeFilter.Add(".jpg");

            var query = contentFolder.CreateFileQueryWithOptions(queryOptions);
            var files = await query.GetFilesAsync();

            return files.Select(CreatePhoto).ToList();
        }

        private Photo CreatePhoto(StorageFile content)
        {
            StorageFileReference sidecar = new StorageFileReference(sidecarFolder, content.DisplayName + ".dat");
            return new Photo(content, sidecar);
        }
    }
}
