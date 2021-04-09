using Fotografix.Drawing;

namespace Fotografix.Editor.Drawing
{
    public sealed record DrawCommand(BitmapLayer Layer, IDrawable Drawable);
}
