using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Export
{
    public sealed class NullExportHandler : IExportHandler
    {
        public Task ExportAsync(IEnumerable<IExportable> items, bool showDialog)
        {
            return Task.CompletedTask;
        }
    }
}
