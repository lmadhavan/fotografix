using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D.Composition
{
    internal sealed class BrushStrokeNode : IDisposable
    {
        private readonly BrushStroke brushStroke;
        private readonly ICanvasResourceCreator resourceCreator;
        private readonly CompositeEffectNode compositeEffectNode;
        private readonly BrushStrokePainter brushStrokePainter;

        private CanvasCommandList commandList;

        internal BrushStrokeNode(BrushStroke brushStroke)
        {
            this.brushStroke = brushStroke;
            this.resourceCreator = CanvasDevice.GetSharedDevice();
            brushStroke.ContentChanged += OnContentChanged;

            this.compositeEffectNode = new CompositeEffectNode();

            this.brushStrokePainter = new BrushStrokePainter(brushStroke);
            UpdateCommandList();
        }

        public void Dispose()
        {
            commandList?.Dispose();
            compositeEffectNode.Dispose();
            brushStroke.ContentChanged -= OnContentChanged;
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
                brushStrokePainter.Paint(ds);
            }

            OutputChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}