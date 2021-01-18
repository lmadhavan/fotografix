using Fotografix.Drawing;

namespace Fotografix.Editor.Drawing
{
    public sealed class DrawCommandHandler : ICommandHandler<DrawCommand>
    {
        private readonly IDrawingContextFactory drawingContextFactory;

        public DrawCommandHandler(IDrawingContextFactory drawingContextFactory)
        {
            this.drawingContextFactory = drawingContextFactory;
        }

        public void Handle(DrawCommand command)
        {
            Draw(command.Bitmap, command.Drawable);
        }

        private void Draw(Bitmap bitmap, IDrawable drawable)
        {
            using (IDrawingContext dc = drawingContextFactory.CreateDrawingContext(bitmap))
            {
                drawable.Draw(dc);
            }
        }
    }
}
