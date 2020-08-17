using System.Drawing;

namespace Fotografix.Drawing
{
    public interface IGradientFactory
    {
        IGradient CreateLinearGradient(Color startColor, Color endColor, Point startPoint);
    }
}
