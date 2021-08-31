using System;
using System.Threading.Tasks;

namespace Fotografix
{
    public sealed class NotifyTaskCompletion<TResult> : NotifyPropertyChangedBase
    {
        public NotifyTaskCompletion(Task<TResult> task)
        {
            Task = task;

            if (!task.IsCompleted)
            {
                WatchTaskAsync(task);
            }
        }

        public Task<TResult> Task { get; }

        public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default;
        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !Task.IsCompleted;
        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;

        private async void WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }

            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsNotCompleted));

            if (task.IsCanceled)
            {
                RaisePropertyChanged(nameof(IsCanceled));
            }
            else if (task.IsFaulted)
            {
                RaisePropertyChanged(nameof(IsFaulted));
                RaisePropertyChanged(nameof(Exception));
            }
            else
            {
                RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
                RaisePropertyChanged(nameof(Result));
            }
        }
    }
}
