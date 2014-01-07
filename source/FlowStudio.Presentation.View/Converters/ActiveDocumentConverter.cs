// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.View.Converters
{
    using System;
    using System.Windows.Data;
    using ViewModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ActiveDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is WorkflowFileViewModel)
            {
                return value;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is WorkflowFileViewModel)
            {
                return value;
            }

            return Binding.DoNothing;
        }
    }
}
