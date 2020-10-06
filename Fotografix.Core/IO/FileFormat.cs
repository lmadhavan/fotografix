using System.Collections.Generic;
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

        public bool SupportsFileExtension(string fileExtension)
        {
            return FileExtensions.Contains(fileExtension);
        }

        public override string ToString()
        {
            return Name + " (" + string.Join(";", FileExtensions) + ")";
        }
    }
}