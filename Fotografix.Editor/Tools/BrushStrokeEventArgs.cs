using System;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushStrokeEventArgs : EventArgs
    {
        public BrushStrokeEventArgs(Layer layer, BrushStroke brushStroke)
        {
            this.Layer = layer;
            this.BrushStroke = brushStroke;
        }

        public Layer Layer { get; }
        public BrushStroke BrushStroke { get; }

        public Command CreatePaintCommand()
        {
            return new PaintBrushStrokeCommand(Layer, BrushStroke);
        }
    }
}