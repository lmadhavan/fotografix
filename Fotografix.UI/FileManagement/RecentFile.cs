using Windows.Storage.AccessCache;

namespace Fotografix.UI.FileManagement
{
    public sealed class RecentFile
    {
        private readonly AccessListEntry entry;

        internal RecentFile(AccessListEntry entry)
        {
            this.entry = entry;
        }

        public string DisplayName => entry.Metadata;
        internal string Token => entry.Token;
    }
}
