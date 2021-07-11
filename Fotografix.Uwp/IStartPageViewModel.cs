using Fotografix.Editor;
using Fotografix.Uwp.FileManagement;
using System.Threading.Tasks;

namespace Fotografix.Uwp
{
    public interface IStartPageViewModel
    {
        AsyncCommand NewCommand { get; }
        AsyncCommand OpenCommand { get; }

        RecentFileList RecentFiles { get; }
        Task OpenRecentFileAsync(RecentFile recentFile);
    }
}
