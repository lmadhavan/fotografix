using Fotografix.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed record DrawCommand(BitmapLayer Layer, IDrawable Drawable);
}
