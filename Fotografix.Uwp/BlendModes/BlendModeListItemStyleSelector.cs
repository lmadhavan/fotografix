using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp.BlendModes
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
