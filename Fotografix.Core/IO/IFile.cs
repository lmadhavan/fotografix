using System.IO;
using System.Threading.Tasks;

namespace Fotografix.IO
{
    public interface IFile
    {
        string Name { get; }
        Task<Stream> OpenReadAsync();
        Task<Stream> OpenWriteAsync();
    }
}