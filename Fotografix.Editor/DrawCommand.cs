using Fotografix.Drawing;
using System;

namespace Fotografix.Editor
{
    public sealed class DrawCommand : SynchronousDocumentCommand
    {
        private readonly IGraphicsDevice graphicsDevice;

        public DrawCommand(IGraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public override bool CanExecute(Document document, object parameter)
        {
            return parameter is DrawCommandArgs args
                && args.Channel == document.ActiveChannel
                && args.Channel.CanDraw;
        }

        public override void Execute(Document document, object parameter)
        {
            var args = (DrawCommandArgs)parameter;

            if (args.Channel != document.ActiveChannel)
            {
                throw new InvalidOperationException();
            }

            args.Channel.Draw(args.Drawable, graphicsDevice);
        }
    }

    public sealed record DrawCommandArgs(Channel Channel, IDrawable Drawable);
}
