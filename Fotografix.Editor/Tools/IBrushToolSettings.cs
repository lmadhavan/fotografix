﻿using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public interface IBrushToolSettings
    {
        int Size { get; set; }
        Color Color { get; set; }
    }
}
