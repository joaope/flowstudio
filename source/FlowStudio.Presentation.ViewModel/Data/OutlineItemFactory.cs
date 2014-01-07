// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Data
{
    using System.Activities;
    using System.Activities.Presentation.Model;
    using System.Activities.Statements;

    public static class OutlineItemFactory
    {
        private static void PopulateInnerItemsRecursively(
            ModelItem modelItem,
            OutlineItemObservableCollection propertiesItems)
        {
            if (modelItem == null)
            {
                return;
            }

            foreach (var property in modelItem.Properties)
            {
                if (property == null)
                {
                    continue;
                }

                var propertyHolderOutlineItem = new OutlinePropertyHolderItem(property.Name);

                if (property.IsCollection &&
                    property.Collection != null &&
                    property.PropertyType.IsGenericType)
                {
                    var genericTypes = property.PropertyType.GetGenericArguments();

                    if (genericTypes.Length == 1 &&
                        (genericTypes[0].IsAssignableFrom(typeof (Activity)) ||
                         genericTypes[0].IsAssignableFrom(typeof (Variable)) ||
                         genericTypes[0].IsAssignableFrom(typeof(FlowNode))))
                    {
                        foreach (var modelItemItem in property.Collection)
                        {
                            propertyHolderOutlineItem.InnerItems.Add(GetOutlineItem(modelItemItem));
                        }
                        propertiesItems.Add(propertyHolderOutlineItem);
                    }
                }
                else
                {
                    var primitive = GetOutlineItem(property.Value);

                    if (primitive != null)
                    {
                        propertyHolderOutlineItem.InnerItems.Add(primitive);
                        propertiesItems.Add(propertyHolderOutlineItem);
                    }
                }
            }
        }

        public static OutlineItem GetOutlineItem(ModelItem modelItem)
        {
            if (modelItem == null)
            {
                return null;
            }

            if (modelItem.ItemType.IsAssignableFrom(typeof(Variable)) ||
                modelItem.ItemType.IsSubclassOf(typeof(Variable)))
            {
                var variable = modelItem.GetCurrentValue() as Variable;

                if (variable != null)
                {
                    return new OutlineVariableItem(variable.Name, variable.Type);
                }
            }
            else if (modelItem.ItemType.IsAssignableFrom(typeof (Activity)) ||
                     modelItem.ItemType.IsSubclassOf(typeof(Activity)))
            {
                var activity = modelItem.GetCurrentValue() as Activity;

                if (activity != null)
                {
                    var activityItem = new OutlineActivityItem(activity.DisplayName, activity.Id, modelItem.ItemType);
                    PopulateInnerItemsRecursively(modelItem, activityItem.InnerItems);
                    return activityItem;
                }
            }
            else if (modelItem.ItemType.IsAssignableFrom(typeof (ActivityBuilder)))
            {
                var builder = modelItem.GetCurrentValue() as ActivityBuilder;

                if (builder != null)
                {
                    var builderItem = new OutlineActivityBuilderItem(builder.Name);
                    PopulateInnerItemsRecursively(modelItem, builderItem.InnerItems);
                    return builderItem;
                }
            }
            else if (modelItem.ItemType.IsAssignableFrom(typeof (FlowNode)) ||
                     modelItem.ItemType.IsSubclassOf(typeof(FlowNode)))
            {
                var flowNode = modelItem.GetCurrentValue() as FlowNode;

                if (flowNode != null)
                {
                    var activityItem = new OutlineFlowNodeItem(modelItem.ItemType);
                    PopulateInnerItemsRecursively(modelItem, activityItem.InnerItems);
                    return activityItem;
                }
            }

            return null;
        }
    }
}
