using Fotografix.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed record DrawCommand(Layer Layer, IDrawable Drawable);
}
