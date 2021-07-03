using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public interface IDocumentCommand
    {
        bool CanExecute(Document document);
        Task ExecuteAsync(Document document);
    }
}
