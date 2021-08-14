using Fotografix.Editor.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class Workspace : NotifyPropertyChangedBase
    {
        private readonly HashSet<Document> documents = new();
        private Document activeDocument;
        private ITool activeTool;

        public Document ActiveDocument
        {
            get => activeDocument;

            set
            {
                Document previous = activeDocument;

                if (SetProperty(ref activeDocument, value))
                {
                    if (previous != null)
                    {
                        previous.ContentChanged -= ActiveDocument_ContentChanged;
                        activeTool?.Deactivated();
                    }

                    if (activeDocument != null)
                    {
                        activeTool?.Activated(activeDocument);
                        activeDocument.ContentChanged += ActiveDocument_ContentChanged;
                    }

                    RequerySuggested?.Invoke(this, EventArgs.Empty);
                }

            }
        }

        public IReadOnlyCollection<Document> Documents => documents;

        public ITool ActiveTool
        {
            get => activeTool;

            set
            {
                ITool previous = activeTool;

                if (SetProperty(ref activeTool, value))
                {
                    if (activeDocument != null)
                    {
                        previous?.Deactivated();
                        activeTool?.Activated(activeDocument);
                    }
                }
            }
        }

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

        public AsyncCommand Bind(EditorCommand command)
        {
            return new EditorCommandAdapter(command, this);
        }

        private sealed class EditorCommandAdapter : AsyncCommand
        {
            private readonly EditorCommand editorCommand;
            private readonly Workspace workspace;

            public EditorCommandAdapter(EditorCommand editorCommand, Workspace workspace)
            {
                this.editorCommand = editorCommand;
                this.workspace = workspace;
            }

            public override event EventHandler CanExecuteChanged
            {
                add => workspace.RequerySuggested += value;
                remove => workspace.RequerySuggested -= value;
            }

            public override bool CanExecute(object parameter)
            {
                return editorCommand.CanExecute(workspace, parameter);
            }

            public override Task ExecuteAsync(object parameter)
            {
                Debug.WriteLine($"Executing {editorCommand.GetType().Name} with parameter [{parameter}]");
                return editorCommand.ExecuteAsync(workspace, parameter);
            }
        }
    }
}
