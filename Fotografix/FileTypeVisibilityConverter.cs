using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Fotografix
{
    public sealed class FileTypeVisibilityConverter : IValueConverter
    {
        private string rawFileTypes;
        private string[] fileTypes;

        public string VisibleFileTypes
        {
            get => rawFileTypes;

            set
            {
                this.rawFileTypes = value;
                this.fileTypes = rawFileTypes.Split(';');
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string filename)
            {
                string extension = Path.GetExtension(filename);

                if (Array.IndexOf(fileTypes, extension) > -1)
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
