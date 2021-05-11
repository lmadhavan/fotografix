using Fotografix.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed record DrawCommand(Channel Channel, IDrawable Drawable);
}
