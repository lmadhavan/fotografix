using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D.Composition
{
    internal sealed class DrawableNode : IDisposable
    {
        private readonly IWin2DDrawable drawable;
        private readonly ICanvasResourceCreator resourceCreator;
        private readonly CompositeEffectNode compositeEffectNode;

        private CanvasCommandList commandList;

        internal DrawableNode(IDrawable drawable)
        {
            this.drawable = (IWin2DDrawable)drawable;
            this.resourceCreator = CanvasDevice.GetSharedDevice();
            drawable.ContentChanged += OnContentChanged;

            this.compositeEffectNode = new CompositeEffectNode();

            UpdateCommandList();
        }

        public void Dispose()
        {
            commandList?.Dispose();
            compositeEffectNode.Dispose();
            drawable.ContentChanged -= OnContentChanged;
        }

        public event EventHandler OutputChanged;

        public ICanvasImage ResolveOutput(ICanvasImage background)
        {
            return compositeEffectNode.ResolveOutput(commandList, background);
        }

        private void OnContentChanged(object sender, EventArgs e)
        {
            UpdateCommandList();
        }

        private void UpdateCommandList()
        {
            commandList?.Dispose();

            this.commandList = new CanvasCommandList(resourceCreator);
            using (CanvasDrawingSession ds = commandList.CreateDrawingSession())
            {
                drawable.Draw(ds);
            }

            OutputChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}