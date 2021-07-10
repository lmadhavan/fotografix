using System;

namespace Fotografix.Editor
{
    public sealed class DocumentEventArgs : EventArgs
    {
        public DocumentEventArgs(Document document)
        {
            this.Document = document;
        }

        public Document Document { get; }
    }
}