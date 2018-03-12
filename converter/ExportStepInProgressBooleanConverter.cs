using System;
using WIMEX.Model;
using Windows.UI.Xaml.Data;

namespace WIMEX.Converter
{
    public class ExportStepInProgressBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.Equals(ExportStepState.InProgess);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}