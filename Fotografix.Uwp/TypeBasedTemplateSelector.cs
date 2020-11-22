using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp
{
    public sealed class TypeBasedTemplateSelector : DataTemplateSelector
    {
        private static readonly DataTemplate EmptyTemplate = new DataTemplate();

        private readonly Dictionary<Type, DataTemplate> templates = new Dictionary<Type, DataTemplate>();

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item == null)
            {
                return null;
            }

            return templates.GetValueOrDefault(item.GetType(), EmptyTemplate);
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
