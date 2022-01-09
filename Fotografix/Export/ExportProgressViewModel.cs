using System;
using System.Threading;

namespace Fotografix.Export
{
    public sealed class ExportProgressViewModel : NotifyPropertyChangedBase, IProgress<ExportProgress>
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        public ExportProgressViewModel(CancellationTokenSource cancellationTokenSource)
        {
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public bool IsActive { get; private set; }
        public int TotalItems { get; private set; }
        public int CompletedItems { get; private set; }
        public string Status => $"{CompletedItems} of {TotalItems} completed";

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        public void Report(ExportProgress value)
        {
            this.IsActive = true;
            this.TotalItems = value.TotalItems;
            this.CompletedItems = value.CompletedItems;

            RaiseAllPropertiesChanged();
        }
    }
}
