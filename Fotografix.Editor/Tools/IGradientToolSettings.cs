using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public interface IGradientToolSettings
    {
        Color EndColor { get; set; }
        Color StartColor { get; set; }
    }
}