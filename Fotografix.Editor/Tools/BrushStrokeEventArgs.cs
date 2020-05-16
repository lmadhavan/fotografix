using System;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushStrokeEventArgs : EventArgs
    {
        public BrushStrokeEventArgs(BrushStroke brushStroke)
        {
            this.BrushStroke = brushStroke;
        }

        public BrushStroke BrushStroke { get; }
    }
}