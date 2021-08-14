using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public abstract class DocumentCommand : EditorCommand
    {
        public sealed override bool CanExecute(Workspace workspace, object parameter)
        {
            return workspace.ActiveDocument != null && CanExecute(workspace.ActiveDocument, parameter);
        }

        public sealed override Task ExecuteAsync(Workspace workspace, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            return ExecuteInChangeGroupAsync(workspace.ActiveDocument, parameter, cancellationToken, progress);
        }

        public bool CanExecute(Document document)
        {
            return CanExecute(document, null);
        }

        public virtual bool CanExecute(Document document, object parameter)
        {
            return true;
        }

        public async Task ExecuteInChangeGroupAsync(Document document, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            using (document.BeginChangeGroup())
            {
                await ExecuteAsync(document, parameter, cancellationToken, progress);
            }
        }

        public Task ExecuteAsync(Document document)
        {
            return ExecuteAsync(document, null);
        }

        public Task ExecuteAsync(Document document, object parameter)
        {
            return ExecuteAsync(document, parameter, default, null);
        }

        public abstract Task ExecuteAsync(Document document, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress);
    }
}
