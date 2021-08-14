namespace Fotografix.Editor.ChangeTracking
{
    public sealed class UndoCommand : SynchronousDocumentCommand
    {
        public override bool CanExecute(Document document, object parameter)
        {
            return document.CanUndo;
        }

        public override void Execute(Document document, object parameter)
        {
            document.Undo();
        }
    }
}
