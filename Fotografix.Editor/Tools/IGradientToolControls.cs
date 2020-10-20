using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public interface IGradientToolControls
    {
        Color StartColor { get; set; }
        Color EndColor { get; set; }
    }
}