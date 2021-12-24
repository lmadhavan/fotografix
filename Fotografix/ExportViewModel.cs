using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix
{
    public sealed class ExportViewModel : NotifyPropertyChangedBase
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        private StorageFolder destinationFolder;
        private bool putInSubfolder;
        private string subfolderName;

        private bool valid;
        private bool subfolderNameInvalid;

        public ExportViewModel(StorageFolder destinationFolder)
        {
            this.DestinationFolder = destinationFolder;
            this.SubfolderName = "Export";
            this.PutInSubfolder = true;
            this.InvalidFileNameMessage = "File names cannot contain the following characters: " + string.Join(' ', InvalidFileNameChars.Where(c => !char.IsControl(c)));
            
            Validate();
        }

        public StorageFolder DestinationFolder
        {
            get => destinationFolder;
            set => SetProperty(ref destinationFolder, value ?? throw new ArgumentNullException());
        }

        public bool PutInSubfolder
        {
            get => putInSubfolder;

            set
            {
                if (SetProperty(ref putInSubfolder, value))
                {
                    Validate();
                }
            }
        }

        public string SubfolderName
        {
            get => subfolderName;

            set
            {
                if (SetProperty(ref subfolderName, value ?? throw new ArgumentNullException()))
                {
                    Validate();
                }
            }
        }

        public async Task<ExportOptions> CreateExportOptionsAsync()
        {
            var folder = destinationFolder;

            if (putInSubfolder)
            {
                folder = await folder.CreateFolderAsync(subfolderName, CreationCollisionOption.OpenIfExists);
            }

            return new ExportOptions(folder);
        }

        #region Validation

        public bool IsValid
        {
            get => valid;
            private set => SetProperty(ref valid, value);
        }

        public bool IsSubfolderNameInvalid
        {
            get => subfolderNameInvalid;
            private set => SetProperty(ref subfolderNameInvalid, value);
        }

        public string InvalidFileNameMessage { get; }

        private void Validate()
        {
            this.IsSubfolderNameInvalid = subfolderName.IndexOfAny(InvalidFileNameChars) > -1;
            this.IsValid = !putInSubfolder || (subfolderName.Trim().Length > 0 && !subfolderNameInvalid);
        }

        #endregion
    }
}
