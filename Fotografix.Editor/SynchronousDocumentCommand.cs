using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public abstract class SynchronousDocumentCommand : DocumentCommand
    {
        public sealed override Task ExecuteAsync(Document document, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            Execute(document, parameter);
            return Task.CompletedTask;
        }

        public void Execute(Document document)
        {
            Execute(document, null);
        }

        public abstract void Execute(Document document, object parameter);
    }
}
