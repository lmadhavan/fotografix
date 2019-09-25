using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Editor.Adjustments.UI
{
    public sealed class AdjustmentTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> templates = new Dictionary<Type, DataTemplate>();

        public DataTemplate BlackAndWhiteTemplate
        {
            get
            {
                return GetTemplate<BlackAndWhiteAdjustment>();
            }

            set
            {
                SetTemplate<BlackAndWhiteAdjustment>(value);
            }
        }

        public DataTemplate ShadowsHighlightsTemplate
        {
            get
            {
                return GetTemplate<ShadowsHighlightsAdjustment>();
            }

            set
            {
                SetTemplate<ShadowsHighlightsAdjustment>(value);
            }
        }

        public DataTemplate HueSaturationTemplate
        {
            get
            {
                return GetTemplate<HueSaturationAdjustment>();
            }

            set
            {
                SetTemplate<HueSaturationAdjustment>(value);
            }
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item == null ? null : templates[item.GetType()];
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        private DataTemplate GetTemplate<T>()
        {
            return templates.GetValueOrDefault(typeof(T));
        }

        private void SetTemplate<T>(DataTemplate template)
        {
            templates[typeof(T)] = template;
        }
    }
}
