// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel.Data
{
    using System;

    public class OutlineActivityItem : OutlineItem
    {
        public string DisplayName { get; private set; }

        public string Id { get; private set; }

        public Type ActivityType { get; private set; }

        public OutlineActivityItem(string displayName, string id, Type activityType)
        {
            DisplayName = displayName;
            Id = id;
            ActivityType = activityType;
        }
    }
}
