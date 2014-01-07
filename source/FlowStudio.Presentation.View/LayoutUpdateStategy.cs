// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModel;
    using Xceed.Wpf.AvalonDock.Layout;

    public class LayoutUpdateStategy : ILayoutUpdateStrategy
    {
        private static readonly List<Tuple<Type, string, bool>> ViewModelPanes = new List<Tuple<Type, string, bool>>
        {
            new Tuple<Type, string, bool>(typeof(PropertyInspectorViewModel), "RightToolsPane", false),
            new Tuple<Type, string, bool>(typeof(ToolboxViewModel), "LeftToolsPane", false),
            new Tuple<Type, string, bool>(typeof(WorkflowOutlineViewModel), "RightToolsPane", false),
            new Tuple<Type, string, bool>(typeof(ErrorsListViewModel), "BottomToolsPane", false),
            new Tuple<Type, string, bool>(typeof(OutputViewModel), "BottomToolsPane", false),
            new Tuple<Type, string, bool>(typeof(DebugViewModel), "BottomToolsPane", false)
        };

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }

        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            if (destinationContainer != null &&
                destinationContainer.FindParent<LayoutFloatingWindow>() != null)
            {
                return false;
            }

            foreach (var viewModelPane in ViewModelPanes)
            {
                if (viewModelPane.Item1.IsInstanceOfType(anchorableToShow.Content))
                {
                    var pane = layout
                        .Descendents()
                        .OfType<LayoutAnchorablePane>()
                        .SingleOrDefault(p => p.Name == viewModelPane.Item2);

                    if (pane != null)
                    {
                        pane.Children.Add(anchorableToShow);

                        if (viewModelPane.Item3)
                        {
                            anchorableToShow.ToggleAutoHide();
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }
    }
}
