namespace Fotografix.Editor.Commands
{
    public sealed class CropCommandHandler : ICommandHandler<CropCommand>
    {
        public void Handle(CropCommand command)
        {
            command.Image.Crop(command.Rectangle);
        }
    }
}
