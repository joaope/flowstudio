// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GalaSoft.MvvmLight.Messaging;

    public class SelectActivitiesTypesMessage : NotificationMessageAction<SelectActivitiesTypesResult>
    {
        public Type[] AvailableActivitiesTypes { get; private set; }

        public SelectActivitiesTypesMessage(
            IEnumerable<Type> availableActivitiesTypes,
            string notification,
            Action<SelectActivitiesTypesResult> callback)
            : base(notification, callback)
        {
            AvailableActivitiesTypes = availableActivitiesTypes.ToArray();
        }

        public SelectActivitiesTypesMessage(
            IEnumerable<Type> availableActivitiesTypes,
            object sender,
            string notification,
            Action<SelectActivitiesTypesResult> callback)
            : base(sender, notification, callback)
        {
            AvailableActivitiesTypes = availableActivitiesTypes == null
                ? new Type[0]
                : availableActivitiesTypes.ToArray();
        }

        public SelectActivitiesTypesMessage(
            IEnumerable<Type> availableActivitiesTypes,
            object sender,
            object target,
            string notification,
            Action<SelectActivitiesTypesResult> callback)
            : base(sender, target, notification, callback)
        {
            AvailableActivitiesTypes = availableActivitiesTypes.ToArray();
        }
    }
}
