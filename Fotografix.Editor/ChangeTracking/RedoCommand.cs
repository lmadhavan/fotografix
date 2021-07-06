using System.Threading.Tasks;

namespace Fotografix.Editor.ChangeTracking
{
    public sealed class RedoCommand : IDocumentCommand
    {
        public bool CanExecute(Document document)
        {
            return document.CanRedo;
        }

        public Task ExecuteAsync(Document document)
        {
            document.Redo();
            return Task.CompletedTask;
        }
    }
}
