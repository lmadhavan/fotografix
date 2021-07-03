using System;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class DocumentCommandAdapter : AsyncCommand
    {
        private readonly IDocumentCommand documentCommand;
        private readonly Document document;

        public DocumentCommandAdapter(IDocumentCommand documentCommand, Document document)
        {
            this.documentCommand = documentCommand;
            this.document = document;
        }

        public override event EventHandler CanExecuteChanged
        {
            add { if (documentCommand is IObservableDocumentCommand o) o.CanExecuteChanged += value; }
            remove { if (documentCommand is IObservableDocumentCommand o) o.CanExecuteChanged -= value; }
        }

        public override bool CanExecute()
        {
            return documentCommand.CanExecute(document);
        }

        public override Task ExecuteAsync()
        {
            return documentCommand.ExecuteAsync(document);
        }
    }
}
