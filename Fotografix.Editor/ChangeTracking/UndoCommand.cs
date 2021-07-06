using System.Threading.Tasks;

namespace Fotografix.Editor.ChangeTracking
{
    public sealed class UndoCommand : IDocumentCommand
    {
        public bool CanExecute(Document document)
        {
            return document.CanUndo;
        }

        public Task ExecuteAsync(Document document)
        {
            document.Undo();
            return Task.CompletedTask;
        }
    }
}
