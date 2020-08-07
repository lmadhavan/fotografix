using System;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class GradientTool : ITool, IGradientToolSettings
    {
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public string Name => "Gradient";
        public object Settings => this;
        public ToolCursor Cursor => throw new NotImplementedException();

        public void PointerMoved(PointerState p)
        {
            throw new NotImplementedException();
        }

        public void PointerPressed(PointerState p)
        {
            throw new NotImplementedException();
        }

        public void PointerReleased(PointerState p)
        {
            throw new NotImplementedException();
        }
    }
}
