using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Export
{
    public interface IExportHandler
    {
        Task ExportAsync(IEnumerable<IExportable> items, bool showDialog);
    }
}