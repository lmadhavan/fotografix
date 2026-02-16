using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Export
{
    public sealed class NullExportHandler : IExportHandler
    {
        public Task ExportAsync(IReadOnlyCollection<IExportable> items)
        {
            return Task.CompletedTask;
        }
    }
}
