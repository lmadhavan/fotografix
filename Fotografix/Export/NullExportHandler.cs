using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Export
{
    public sealed class NullExportHandler : IExportHandler
    {
        public Task ExportAsync(IReadOnlyCollection<IExportable> items, bool showDialog, CancellationToken token = default, IProgress<ExportProgress> progress = null)
        {
            return Task.CompletedTask;
        }
    }
}
