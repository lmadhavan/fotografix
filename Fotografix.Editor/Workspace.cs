﻿using Fotografix.Editor.Tools;
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
                        activeTool?.Activated(activeDocument.Image);
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
                        activeTool?.Activated(activeDocument.Image);
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
                Debug.WriteLine("Executing " + workspaceCommand.GetType().Name);
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
                    Debug.WriteLine("Executing " + documentCommand.GetType().Name);
                    await documentCommand.ExecuteAsync(workspace.ActiveDocument);
                }
            }
        }
    }
}
