using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Export
{
    public interface IExportHandler
    {
        Task ExportAsync(IReadOnlyCollection<IExportable> items, bool showDialog, CancellationToken token = default, IProgress<ExportProgress> progress = null);
    }
}