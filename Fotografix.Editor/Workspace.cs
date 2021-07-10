using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class Workspace : NotifyPropertyChangedBase
    {
        private readonly HashSet<Document> documents = new();
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

                SetProperty(ref activeDocument, value);

                if (activeDocument != null)
                {
                    activeDocument.ContentChanged += ActiveDocument_ContentChanged;
                }

                RequerySuggested?.Invoke(this, EventArgs.Empty);
            }
        }

        public IReadOnlyCollection<Document> Documents => documents;

        public event EventHandler<DocumentEventArgs> DocumentAdded;
        public event EventHandler<DocumentEventArgs> DocumentRemoved;

        public void AddDocument(Document document)
        {
            if (documents.Add(document))
            {
                DocumentAdded?.Invoke(this, new DocumentEventArgs(document));
                this.ActiveDocument = document;
            }
        }

        public void RemoveDocument(Document document)
        {
            if (documents.Remove(document))
            {
                DocumentRemoved?.Invoke(this, new DocumentEventArgs(document));

                if (activeDocument == document)
                {
                    this.ActiveDocument = documents.FirstOrDefault();
                }
            }
        }

        private event EventHandler RequerySuggested;

        private void ActiveDocument_ContentChanged(object sender, EventArgs e)
        {
            RequerySuggested?.Invoke(this, EventArgs.Empty);
        }

        public AsyncCommand Bind(IWorkspaceCommand command)
        {
            return new WorkspaceCommandAdapter(command, this);
        }

        public AsyncCommand Bind(IDocumentCommand command)
        {
            return new DocumentCommandAdapter(command, this);
        }

        private sealed class WorkspaceCommandAdapter : AsyncCommand
        {
            private readonly IWorkspaceCommand workspaceCommand;
            private readonly Workspace workspace;

            public WorkspaceCommandAdapter(IWorkspaceCommand workspaceCommand, Workspace workspace)
            {
                this.workspaceCommand = workspaceCommand;
                this.workspace = workspace;
            }

            public override event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }

            public override bool CanExecute()
            {
                return true;
            }

            public override Task ExecuteAsync()
            {
                return workspaceCommand.ExecuteAsync(workspace);
            }
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

            public override async Task ExecuteAsync()
            {
                Document document = workspace.activeDocument;
                using (document.BeginChangeGroup())
                {
                    await documentCommand.ExecuteAsync(workspace.ActiveDocument);
                }
            }
        }
    }
}
