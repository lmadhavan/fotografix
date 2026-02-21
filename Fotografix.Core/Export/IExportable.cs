using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Export
{
    public interface IExportable
    {
        string Name { get; }
        Task<StorageFile> ExportAsync(ExportOptions options);
    }
}