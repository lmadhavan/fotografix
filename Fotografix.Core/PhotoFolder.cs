using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Search;

namespace Fotografix
{
    public sealed class PhotoFolder
    {
        /// <summary>
        /// JPEG files are treated with lowest priority when picking the primary photo in a group of linked photos.
        /// </summary>
        private static readonly IEnumerable<string> DeprioritizedFileExtensions = new string[] { ".jpg", ".jpeg" };

        private readonly StorageFolder contentFolder;
        private readonly StorageFolder sidecarFolder;

        public PhotoFolder(StorageFolder contentFolder, StorageFolder sidecarFolder)
        {
            this.contentFolder = contentFolder;
            this.sidecarFolder = sidecarFolder;
        }

        public Task<IReadOnlyList<Photo>> GetPhotosAsync()
        {
            var fileExtensions = BitmapDecoder.GetDecoderInformationEnumerator().SelectMany(bci => bci.FileExtensions);
            return GetPhotosAsync(fileExtensions);
        }

        public async Task<IReadOnlyList<Photo>> GetPhotosAsync(IEnumerable<string> fileExtensions)
        {
            var queryOptions = new QueryOptions();
            foreach (var extension in fileExtensions)
            {
                queryOptions.FileTypeFilter.Add(extension);
            }

            var query = contentFolder.CreateFileQueryWithOptions(queryOptions);
            var files = await query.GetFilesAsync();
            return GroupPhotos(files.Select(CreatePhoto));
        }

        private IReadOnlyList<Photo> GroupPhotos(IEnumerable<Photo> photos)
        {
            var groupedPhotos = new List<Photo>();

            var groups = photos.GroupBy(BaseName);
            foreach (var group in groups)
            {
                var primaryPhoto = group.FirstOrDefault(IsPrimaryPhotoCandidate) ?? group.First();
                primaryPhoto.LinkedPhotos.AddRange(group.Where(p => p != primaryPhoto));
                groupedPhotos.Add(primaryPhoto);
            }

            return groupedPhotos;
        }

        private Photo CreatePhoto(StorageFile content)
        {
            StorageFileReference sidecar = new StorageFileReference(sidecarFolder, Path.GetFileNameWithoutExtension(content.Name) + ".dat");
            return new Photo(content, sidecar);
        }

        private string BaseName(Photo photo)
        {
            return Path.GetFileNameWithoutExtension(photo.Name);
        }

        private bool IsPrimaryPhotoCandidate(Photo photo)
        {
            return !DeprioritizedFileExtensions.Contains(Path.GetExtension(photo.Name).ToLower());
        }
    }
}
