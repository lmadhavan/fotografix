using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI
{
    public sealed class TypeBasedTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> templates = new Dictionary<Type, DataTemplate>();

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item == null ? null : templates[item.GetType()];
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        public void SetTemplate<T>(DataTemplate template)
        {
            templates[typeof(T)] = template;
        }
    }
}
