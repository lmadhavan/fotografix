using System;
using System.Windows.Input;

namespace Fotografix.Editor
{
    public sealed class DocumentCommandAdapter : ICommand
    {
        private readonly IDocumentCommand documentCommand;
        private readonly Document document;

        public DocumentCommandAdapter(IDocumentCommand documentCommand, Document document)
        {
            this.documentCommand = documentCommand;
            this.document = document;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return documentCommand.CanExecute(document);
        }

        public async void Execute(object parameter)
        {
            await documentCommand.ExecuteAsync(document);
        }
    }
}
