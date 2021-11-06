using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix
{
    public sealed class StubCanvasResourceCreator : ICanvasResourceCreatorWithDpi
    {
        public float Dpi { get; set; } = 96;
        public CanvasDevice Device { get; set; } = CanvasDevice.GetSharedDevice();

        public float ConvertPixelsToDips(int pixels)
        {
            throw new NotImplementedException();
        }

        public int ConvertDipsToPixels(float dips, CanvasDpiRounding dpiRounding)
        {
            throw new NotImplementedException();
        }
    }
}
