using Fotografix.Editor.Tools;

namespace Fotografix.Uwp.Tools
{
    public partial class ToolTemplatesResourceDictionary
    {
        public ToolTemplatesResourceDictionary()
        {
            InitializeComponent();

            toolControlsTemplateSelector.SetTemplate<BrushTool>(brushControlsTemplate);
            toolControlsTemplateSelector.SetTemplate<GradientTool>(gradientControlsTemplate);
            toolControlsTemplateSelector.SetTemplate<HandTool>(emptyControlsTemplate);
        }
    }
}
