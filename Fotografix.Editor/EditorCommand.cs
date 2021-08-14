using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public abstract class EditorCommand
    {
        public bool CanExecute(Workspace workspace)
        {
            return CanExecute(workspace, null);
        }

        public virtual bool CanExecute(Workspace workspace, object parameter)
        {
            return true;
        }

        public Task ExecuteAsync(Workspace workspace)
        {
            return ExecuteAsync(workspace, null);
        }

        public Task ExecuteAsync(Workspace workspace, object parameter)
        {
            return ExecuteAsync(workspace, parameter, default, null);
        }

        public abstract Task ExecuteAsync(Workspace workspace, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress);
    }

    public sealed record EditorCommandProgress(string Description = "", int CompletedItems = 0, int TotalItems = 0);
}
