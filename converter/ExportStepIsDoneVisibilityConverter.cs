using System;
using WIMEX.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WIMEX.Converter
{
    public class ExportStepIsDoneVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.Equals(ExportStepState.Done) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}