using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp.BlendModes
{
    public sealed class BlendModeListItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BlendModeTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return ((BlendModeListItem)item).IsSeparator ? SeparatorTemplate : BlendModeTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
