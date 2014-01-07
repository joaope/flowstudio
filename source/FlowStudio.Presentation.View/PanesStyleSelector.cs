// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.View
{
    using System.Windows;
    using System.Windows.Controls;
    using ViewModel;

    public class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle { get; set; }

        public Style FileStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ToolViewModel)
            {
                return ToolStyle;
            }

            if (item is WorkflowFileViewModel)
            {
                return FileStyle;
            }

            return base.SelectStyle(item, container);
        }
    }
}
