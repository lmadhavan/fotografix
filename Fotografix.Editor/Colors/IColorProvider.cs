using System.Drawing;

namespace Fotografix.Editor.Colors
{
    public interface IColorProvider
    {
        Color ForegroundColor { get; }
        Color BackgroundColor { get; }
    }
}