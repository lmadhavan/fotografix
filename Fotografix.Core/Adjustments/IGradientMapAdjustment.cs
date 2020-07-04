using System.Drawing;

namespace Fotografix.Adjustments
{
    public interface IGradientMapAdjustment : IAdjustment
    {
        Color Shadows { get; set; }
        Color Highlights { get; set; }
    }
}