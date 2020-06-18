using System;
using Windows.UI.Xaml.Data;

namespace Fotografix.UI.BlendModes
{
    public sealed class BlendModeListItemConverter : IValueConverter
    {
        private readonly BlendModeList blendModeList;

        public BlendModeListItemConverter(BlendModeList blendModeList)
        {
            this.blendModeList = blendModeList;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BlendMode blendMode = (BlendMode)value;
            return blendModeList[blendMode];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            BlendModeListItem item = (BlendModeListItem)value;
            return item.BlendMode;
        }
    }
}