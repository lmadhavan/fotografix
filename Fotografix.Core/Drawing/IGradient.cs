using System.Drawing;

namespace Fotografix.Drawing
{
    public interface IGradient : IDrawable
    {
        void SetEndPoint(Point pt);
    }
}
