using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI
{
    public sealed class TypeBasedDataTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> templates = new Dictionary<Type, DataTemplate>();

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item == null)
            {
                return null;
            }

            foreach (var e in templates)
            {
                if (e.Key.IsInstanceOfType(item))
                {
                    return e.Value;
                }
            }

            throw new ArgumentException("No template found for object of type " + item.GetType());
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
