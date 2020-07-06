﻿using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Adjustments
{
    internal static class ColorMatrix
    {
        internal static readonly Matrix5x4 Identity = new Matrix5x4
            { 
                M11 = 1, M12 = 0, M13 = 0, M14 = 0,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M41 = 0, M42 = 0, M43 = 0, M44 = 1,
                M51 = 0, M52 = 0, M53 = 0, M54 = 0
            };
    }
}
