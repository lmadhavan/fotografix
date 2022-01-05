using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Export
{
    public interface IExportable
    {
        Task<StorageFile> ExportAsync(ExportOptions options);
    }
}