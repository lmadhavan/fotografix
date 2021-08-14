using Fotografix.Editor;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Fotografix.Uwp
{
    public sealed class ProgressViewModel : NotifyPropertyChangedBase
    {
        private static readonly TimeSpan VisibilityDelay = TimeSpan.FromSeconds(1);

        private readonly WorkspaceCommandDispatcher dispatcher;
        private bool visible;
        private EditorCommandProgress progress;

        public ProgressViewModel(WorkspaceCommandDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            dispatcher.PropertyChanged += Dispatcher_PropertyChanged;
            dispatcher.ProgressChanged += Dispatcher_ProgressChanged;
        }

        public bool IsVisible
        {
            get => visible;
            private set => SetProperty(ref visible, value);
        }

        public string Description => progress?.Description;
        public int CompletedItems => progress?.CompletedItems ?? 0;
        public int TotalItems => progress?.TotalItems ?? 0;
        public bool IsIndeterminate => TotalItems == 0;

        public void Cancel()
        {
            dispatcher.CancelActiveCommand();
        }

        private void SetProgress(EditorCommandProgress progress)
        {
            this.progress = progress;

            RaisePropertyChanged(nameof(Description));
            RaisePropertyChanged(nameof(CompletedItems));
            RaisePropertyChanged(nameof(TotalItems));
            RaisePropertyChanged(nameof(IsIndeterminate));
        }

        private void Dispatcher_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WorkspaceCommandDispatcher.IsBusy))
            {
                this.progress = null;
                this.IsVisible = false;
            }
        }

        private void Dispatcher_ProgressChanged(object sender, EditorCommandProgress e)
        {
            if (progress == null)
            {
                BeginTracking();
            }
            
            SetProgress(e);
        }

        private async void BeginTracking()
        {
            await Task.Delay(VisibilityDelay);

            if (dispatcher.IsBusy)
            {
                this.IsVisible = true;
            }
        }
    }
}
