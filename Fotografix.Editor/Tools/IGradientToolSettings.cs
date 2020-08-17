using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public interface IGradientToolSettings
    {
        Color StartColor { get; set; }
        Color EndColor { get; set; }
    }
}