using System;
using Windows.UI.Xaml.Data;

namespace WIMEX.Converter
{
    public class NegateBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var visible = (value as bool?) ?? false;

            return !visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var visible = (value as bool?) ?? false;

            return !visible;
        }
    }
}