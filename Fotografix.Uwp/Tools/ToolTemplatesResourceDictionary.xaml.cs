using Fotografix.Editor.Tools;

namespace Fotografix.Uwp.Tools
{
    public partial class ToolTemplatesResourceDictionary
    {
        public ToolTemplatesResourceDictionary()
        {
            InitializeComponent();

            toolControlsTemplateSelector.SetTemplate<BrushTool>(brushControlsTemplate);
            toolControlsTemplateSelector.SetTemplate<CropTool>(cropControlsTemplate);
            toolControlsTemplateSelector.SetTemplate<GradientTool>(emptyControlsTemplate);
            toolControlsTemplateSelector.SetTemplate<HandTool>(emptyControlsTemplate);
        }
    }
}
