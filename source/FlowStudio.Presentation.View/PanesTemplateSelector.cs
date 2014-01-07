// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.View
{
    using System.Windows;
    using System.Windows.Controls;
    using ViewModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WorkflowFileTemplate { get; set; }

        public DataTemplate ToolboxTemplate { get; set; }

        public DataTemplate PropertyInspectorTemplate { get; set; }

        public DataTemplate ErrorsListTemplate { get; set; }

        public DataTemplate OutputTemplate { get; set; }

        public DataTemplate DebugTemplate { get; set; }

        public DataTemplate WorkflowOutlineTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is WorkflowFileViewModel)
            {
                return WorkflowFileTemplate;
            }

            if (item is ToolboxViewModel)
            {
                return ToolboxTemplate;
            }

            if (item is PropertyInspectorViewModel)
            {
                return PropertyInspectorTemplate;
            }

            if (item is ErrorsListViewModel)
            {
                return ErrorsListTemplate;
            }

            if (item is OutputViewModel)
            {
                return OutputTemplate;
            }

            if (item is DebugViewModel)
            {
                return DebugTemplate;
            }

            if (item is WorkflowOutlineViewModel)
            {
                return WorkflowOutlineTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
