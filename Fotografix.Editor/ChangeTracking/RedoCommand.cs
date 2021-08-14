namespace Fotografix.Editor.ChangeTracking
{
    public sealed class RedoCommand : SynchronousDocumentCommand
    {
        public override bool CanExecute(Document document, object parameter)
        {
            return document.CanRedo;
        }

        public override void Execute(Document document, object parameter)
        {
            document.Redo();
        }
    }
}
