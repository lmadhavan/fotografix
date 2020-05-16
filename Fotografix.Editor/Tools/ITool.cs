﻿using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public interface ITool
    {
        object Settings { get; }
        ToolCursor Cursor { get; }

        void LayerActivated(Layer layer);

        void PointerPressed(PointF pt);
        void PointerMoved(PointF pt);
        void PointerReleased(PointF pt);
    }
}
