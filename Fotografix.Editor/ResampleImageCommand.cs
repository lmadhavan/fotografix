using System.Drawing;

namespace Fotografix.Editor
{
    public sealed record ResampleImageCommand(Image Image, Size NewSize) : ICommand;
}