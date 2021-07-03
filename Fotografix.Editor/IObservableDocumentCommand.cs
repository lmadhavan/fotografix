using System;

namespace Fotografix.Editor
{
    public interface IObservableDocumentCommand : IDocumentCommand
    {
        event EventHandler CanExecuteChanged;
    }
}
