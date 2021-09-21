using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix
{
    public sealed class StorageFileReference
    {
        private readonly StorageFolder folder;
        private readonly string name;

        public StorageFileReference(StorageFolder folder, string name)
        {
            this.folder = folder;
            this.name = name;
        }

        public async Task<StorageFile> TryGetFileAsync()
        {
            var item = await folder.TryGetItemAsync(name);

            if (item != null && item.IsOfType(StorageItemTypes.File))
            {
                return (StorageFile)item;
            }

            return null;
        }

        public Task<StorageFile> GetOrCreateFileAsync()
        {
            return folder.CreateFileAsync(name, CreationCollisionOption.OpenIfExists).AsTask();
        }

        public async Task DeleteAsync()
        {
            var file = await TryGetFileAsync();

            if (file != null)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }
    }
}
