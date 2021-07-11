using Fotografix.Editor;
using Fotografix.Uwp.FileManagement;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Uwp
{
    public sealed class StartPageViewModel
    {
        public AsyncCommand NewCommand { get; set; }
        public AsyncCommand OpenCommand { get; set; }

        public RecentFileList RecentFiles { get; set; }
        public FilePickerOverride FilePickerOverride { get; set; }

        public async Task OpenRecentFileAsync(RecentFile recentFile)
        {
            StorageFile storageFile = await RecentFiles.GetFileAsync(recentFile);

            using (FilePickerOverride.OverrideOpenFile(new StorageFileAdapter(storageFile)))
            {
                await OpenCommand.ExecuteAsync();
            }
        }
    }
}
