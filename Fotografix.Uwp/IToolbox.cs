using Fotografix.Editor.Tools;
using System.Collections.Generic;

namespace Fotografix.Uwp
{
    public interface IToolbox
    {
        IList<ITool> Tools { get; }
        ITool ActiveTool { get; set; }
    }
}
