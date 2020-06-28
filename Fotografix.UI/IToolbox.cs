using Fotografix.Editor.Tools;
using System.Collections.Generic;

namespace Fotografix.UI
{
    public interface IToolbox
    {
        IList<ITool> Tools { get; }
        ITool ActiveTool { get; set; }
    }
}
