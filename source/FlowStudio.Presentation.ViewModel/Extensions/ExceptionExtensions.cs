// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel.Extensions
{
    using System;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ExceptionExtensions
    {
        public static string FormattedStackTrace(this Exception exception)
        {
            var sb = new StringBuilder();
            sb.Append(exception.Message);
            sb.Append(exception.StackTrace);

            if (exception.InnerException != null)
            {
                sb.Append(exception.InnerException.FormattedStackTrace());
            }

            return sb.ToString();
        }
    }
}
