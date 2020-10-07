using System.IO;
using System.Threading.Tasks;

namespace Fotografix.IO
{
    public sealed class InMemoryFile : IFile
    {
        private readonly Stream stream;

        public InMemoryFile(string name)
        {
            this.Name = name;
            this.stream = new MemoryStream();
        }

        public string Name { get; }

        public Task<Stream> OpenReadAsync()
        {
            return Task.FromResult(stream);
        }

        public Task<Stream> OpenWriteAsync()
        {
            return Task.FromResult(stream);
        }
    }
}
