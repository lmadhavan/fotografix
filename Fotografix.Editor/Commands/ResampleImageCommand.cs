using System.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed record ResampleImageCommand(Image Image, Size NewSize);
}