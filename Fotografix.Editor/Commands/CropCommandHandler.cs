using System.Threading.Tasks;

namespace Fotografix.Editor.Commands
{
    public sealed class CropCommandHandler : ICommandHandler<CropCommand>
    {
        public Task HandleAsync(CropCommand command)
        {
            command.Image.Crop(command.Rectangle);
            return Task.CompletedTask;
        }
    }
}
