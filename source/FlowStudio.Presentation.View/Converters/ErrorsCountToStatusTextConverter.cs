// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.View.Converters
{
    using System.Windows.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ErrorsCountToStatusTextConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var i = (value as int?).GetValueOrDefault();

            return i > 0
                       ? string.Format("Errors: {0}", i)
                       : "No Errors";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
