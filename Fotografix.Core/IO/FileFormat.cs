using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fotografix.IO
{
    public sealed class FileFormat
    {
        public FileFormat(string name, IEnumerable<string> fileExtensions)
        {
            this.Name = name;
            this.FileExtensions = fileExtensions;
        }

        public string Name { get; }
        public IEnumerable<string> FileExtensions { get; }

        public bool Matches(IFile file)
        {
            string fileExtension = Path.GetExtension(file.Name);
            return FileExtensions.Contains(fileExtension);
        }

        public override string ToString()
        {
            return Name + " (" + string.Join(";", FileExtensions) + ")";
        }
    }
}