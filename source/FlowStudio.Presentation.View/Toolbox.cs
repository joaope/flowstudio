// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.View
{
    using System.Activities.Presentation.Toolbox;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public sealed class Toolbox : UserControl
    {
        private readonly ToolboxControl toolbox;

        public Toolbox()
        {
            toolbox = new ToolboxControl();
            Content = toolbox;
        }

        public static readonly DependencyProperty CategoriesProperty = DependencyProperty.Register(
            "Categories",
            typeof(ObservableCollection<ToolboxCategory>),
            typeof(Toolbox),
            new PropertyMetadata(default(ObservableCollection<ToolboxCategory>), CategoriesPropertyChangedCallback));

        private static void CategoriesPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var toolboxObject = dependencyObject as Toolbox;

            if (toolboxObject == null)
            {
                return;
            }

            var categories = e.OldValue as ObservableCollection<ToolboxCategory>;

            if (categories != null)
            {
                categories.CollectionChanged -= toolboxObject.OnCollectionChanged;
            }

            categories = e.NewValue as ObservableCollection<ToolboxCategory>;

            if (categories != null)
            {
                categories.CollectionChanged += toolboxObject.OnCollectionChanged;
            }

            toolboxObject.toolbox.Categories = null;

            foreach (var toolboxCategory in categories)
            {
                toolboxObject.toolbox.Categories.Add(toolboxCategory);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var category in e.NewItems.OfType<ToolboxCategory>())
                        {
                            toolbox.Categories.Add(category);
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (
                            var category in
                                e.OldItems.Cast<ToolboxCategory>().Where(category => toolbox.Categories.Contains(category)))
                        {
                            toolbox.Categories.Remove(category);
                        }
                        break;
                    }

                default:
                    {
                        toolbox.Categories = null;

                        foreach (var category in Categories)
                        {
                            toolbox.Categories.Add(category);
                        }
                        break;
                    }
            }
        }

        public IEnumerable<ToolboxCategory> Categories
        {
            get { return (IEnumerable<ToolboxCategory>)GetValue(CategoriesProperty); }
            set { SetValue(CategoriesProperty, value); }
        }
    }
}
