// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.View.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class IsWarningToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var errorInfo = value as bool?;

            return errorInfo.GetValueOrDefault()
                       ? "images\\bullet_orange.png"
                       : "images\\bullet_red.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
