using Fotografix.Drawing;

namespace Fotografix.Editor.Drawing
{
    public sealed class DrawCommand : Command
    {
        private readonly Bitmap bitmap;
        private readonly IDrawingContextFactory drawingContextFactory;
        private readonly IDrawable drawable;

        public DrawCommand(Bitmap bitmap, IDrawingContextFactory drawingContextFactory, IDrawable drawable)
        {
            this.bitmap = bitmap;
            this.drawingContextFactory = drawingContextFactory;
            this.drawable = drawable;
        }

        public override void Execute()
        {
            using (IDrawingContext dc = drawingContextFactory.CreateDrawingContext(bitmap))
            {
                drawable.Draw(dc);
            }
        }
    }
}
