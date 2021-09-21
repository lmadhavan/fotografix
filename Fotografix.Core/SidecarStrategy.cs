using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix
{
    public interface ISidecarStrategy
    {
        Task<StorageFolder> GetSidecarFolderAsync(StorageFolder folder);
    }

    public sealed class SubfolderSidecarStrategy : ISidecarStrategy
    {
        private readonly string subfolderName;

        public SubfolderSidecarStrategy(string subfolderName)
        {
            this.subfolderName = subfolderName;
        }

        public Task<StorageFolder> GetSidecarFolderAsync(StorageFolder folder)
        {
            return folder.CreateFolderAsync(subfolderName, CreationCollisionOption.OpenIfExists).AsTask();
        }
    }

    public sealed class FixedSidecarStrategy : ISidecarStrategy
    {
        private readonly StorageFolder sidecarFolder;

        public FixedSidecarStrategy(StorageFolder sidecarFolder)
        {
            this.sidecarFolder = sidecarFolder;
        }

        public Task<StorageFolder> GetSidecarFolderAsync(StorageFolder folder)
        {
            return Task.FromResult(sidecarFolder);
        }
    }
}
