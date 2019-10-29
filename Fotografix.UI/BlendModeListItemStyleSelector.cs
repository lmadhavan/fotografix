using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI
{
    public sealed class BlendModeListItemStyleSelector : StyleSelector
    {
        public Style SeparatorStyle { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            return ((BlendModeListItem)item).IsSeparator ? SeparatorStyle : null;
        }
    }
}
