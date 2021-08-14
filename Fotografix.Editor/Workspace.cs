using Fotografix.Editor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        activeTool?.Deactivated();
                    }

                    if (activeDocument != null)
                    {
                        activeTool?.Activated(activeDocument);
                    }
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
    }
}
