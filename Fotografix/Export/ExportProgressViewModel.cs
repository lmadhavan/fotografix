using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;
using Windows.UI.Xaml;

namespace Fotografix.Export
{
    public sealed class ExportProgressViewModel : NotifyPropertyChangedBase, IProgress<ExportProgress>
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        public ExportProgressViewModel(CancellationTokenSource cancellationTokenSource)
        {
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public bool IsOpen { get; private set; }
        public bool CanClose { get; private set; }
        public Visibility CancelButtonVisibility { get; private set; }
        public InfoBarSeverity Severity { get; private set; }

        public int TotalItems { get; private set; }
        public int ProcessedItems { get; private set; }
        public int FailedItems { get; private set; }

        private bool IsComplete => TotalItems == ProcessedItems;
        private bool IsCancelled => cancellationTokenSource.IsCancellationRequested;

        public string Status
        {
            get
            {
                string text = $"{ProcessedItems} of {TotalItems} processed";

                if (IsCancelled)
                {
                    text += " (cancelled)";
                }
                else if (FailedItems > 0)
                {
                    text += $" ({FailedItems} failed)";
                }

                return text;
            }
        }

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
            Update();
        }

        public void Report(ExportProgress value)
        {
            this.IsOpen = true;
            this.TotalItems = value.TotalItems;
            this.ProcessedItems = value.ProcessedItems;
            this.FailedItems = value.FailedItems;
            Update();
        }

        private void Update()
        {
            this.CanClose = IsComplete || IsCancelled;
            this.CancelButtonVisibility = CanClose ? Visibility.Collapsed : Visibility.Visible;

            if (FailedItems > 0)
            {
                this.Severity = InfoBarSeverity.Error;
            }
            else if (IsCancelled)
            {
                this.Severity = InfoBarSeverity.Warning;
            }
            else if (IsComplete)
            {
                this.Severity = InfoBarSeverity.Success;
            }
            else
            {
                this.Severity = InfoBarSeverity.Informational;
            }

            RaiseAllPropertiesChanged();
        }
    }
}
