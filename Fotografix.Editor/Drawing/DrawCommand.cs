using Fotografix.Drawing;

namespace Fotografix.Editor.Drawing
{
    public sealed class DrawCommand : ICommand
    {
        public DrawCommand(Bitmap bitmap, IDrawable drawable)
        {
            this.Bitmap = bitmap;
            this.Drawable = drawable;
        }

        public Bitmap Bitmap { get; }
        public IDrawable Drawable { get; }
    }
}
