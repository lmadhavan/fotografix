using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Editor.UI
{
    public sealed class LayerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AdjustmentLayerTemplate { get; set; }
        public DataTemplate DefaultLayerTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is AdjustmentLayer ? AdjustmentLayerTemplate : DefaultLayerTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
