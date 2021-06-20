using Fotografix.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Editor.Commands
{
    public sealed class SaveCommandHandler : ICommandHandler<SaveCommand>, ICommandHandler<SaveAsCommand>
    {
        private readonly IImageEncoder imageEncoder;
        private readonly IFilePicker filePicker;

        public SaveCommandHandler(IImageEncoder imageEncoder, IFilePicker filePicker)
        {
            this.imageEncoder = imageEncoder;
            this.filePicker = filePicker;
        }

        public Task HandleAsync(SaveCommand command)
        {
            return HandleAsync(command.Image, command.Image.GetFile());
        }

        public Task HandleAsync(SaveAsCommand command)
        {
            return HandleAsync(command.Image, null);
        }

        private async Task HandleAsync(Image image, IFile file)
        {
            if (file == null || FindFileFormat(file) == null)
            {
                file = await filePicker.PickSaveFileAsync(imageEncoder.SupportedFileFormats);
            }

            if (file == null)
            {
                return;
            }

            await imageEncoder.WriteImageAsync(image, file, FindFileFormat(file));
            image.SetFile(file);
            image.SetDirty(false);
        }

        private FileFormat FindFileFormat(IFile file)
        {
            return imageEncoder.SupportedFileFormats.FirstOrDefault(f => f.Matches(file));
        }
    }
}
