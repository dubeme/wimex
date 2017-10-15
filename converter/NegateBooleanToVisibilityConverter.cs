using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WIMEX.Converter
{
    public class NegateBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var visible = (value as bool?) ?? false;

            return visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}