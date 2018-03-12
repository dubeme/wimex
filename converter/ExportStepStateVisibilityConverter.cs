using System;
using WIMEX.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WIMEX.Converter
{
    public class ExportStepStateVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Enum.TryParse(value.ToString(), out ExportStepState actualState);
            Enum.TryParse((string)parameter, out ExportStepState expectedState);

            return actualState == expectedState ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}