using System;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class Workspace
    {
        private Document activeDocument;

        public Document ActiveDocument
        {
            get => activeDocument;

            set
            {
                if (activeDocument != null)
                {
                    activeDocument.ContentChanged -= ActiveDocument_ContentChanged;
                }

                this.activeDocument = value;

                if (activeDocument != null)
                {
                    activeDocument.ContentChanged += ActiveDocument_ContentChanged;
                }

                RequerySuggested?.Invoke(this, EventArgs.Empty);
            }
        }

        private event EventHandler RequerySuggested;

        private void ActiveDocument_ContentChanged(object sender, EventArgs e)
        {
            RequerySuggested?.Invoke(this, EventArgs.Empty);
        }

        public AsyncCommand Bind(IDocumentCommand command)
        {
            return new DocumentCommandAdapter(command, this);
        }

        private sealed class DocumentCommandAdapter : AsyncCommand
        {
            private readonly IDocumentCommand documentCommand;
            private readonly Workspace workspace;

            public DocumentCommandAdapter(IDocumentCommand documentCommand, Workspace workspace)
            {
                this.documentCommand = documentCommand;
                this.workspace = workspace;
            }

            public override event EventHandler CanExecuteChanged
            {
                add
                {
                    workspace.RequerySuggested += value;

                    if (documentCommand is IObservableDocumentCommand o)
                    {
                        o.CanExecuteChanged += value;
                    }
                }

                remove
                {
                    workspace.RequerySuggested -= value;

                    if (documentCommand is IObservableDocumentCommand o)
                    {
                        o.CanExecuteChanged -= value;
                    }
                }
            }

            public override bool CanExecute()
            {
                return workspace.ActiveDocument != null && documentCommand.CanExecute(workspace.ActiveDocument);
            }

            public override Task ExecuteAsync()
            {
                return documentCommand.ExecuteAsync(workspace.ActiveDocument);
            }
        }
    }
}
