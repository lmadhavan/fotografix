using Fotografix.Editor.Tools;

namespace Fotografix.UI.Tools
{
    public partial class ToolTemplatesResourceDictionary
    {
        public ToolTemplatesResourceDictionary()
        {
            InitializeComponent();

            toolSettingsTemplateSelector.SetTemplate<BrushTool>(brushToolSettingsTemplate);
            toolSettingsTemplateSelector.SetTemplate<GradientTool>(gradientToolSettingsTemplate);
        }
    }
}
