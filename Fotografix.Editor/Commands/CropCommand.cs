using System.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed record CropCommand(Image Image, Rectangle Rectangle);
}