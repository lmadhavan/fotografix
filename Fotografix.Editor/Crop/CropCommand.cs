using System.Drawing;

namespace Fotografix.Editor.Crop
{
    public sealed record CropCommand(Image Image, Rectangle Rectangle) : ICommand;
}