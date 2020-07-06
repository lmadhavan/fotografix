using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas;

namespace Fotografix.Win2D.Adjustments
{
    internal sealed class Win2DAdjustmentContext : IAdjustmentContext
    {
        public Win2DAdjustmentContext(ICanvasImage input)
        {
            this.Input = input;
        }

        public ICanvasImage Input { get; }
        public ICanvasImage Output { get; set; }
    }
}
