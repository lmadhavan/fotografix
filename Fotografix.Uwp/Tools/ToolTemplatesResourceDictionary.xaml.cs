using Fotografix.Editor.Tools;

namespace Fotografix.Uwp.Tools
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
