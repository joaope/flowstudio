// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Data
{
    using System;
    using GalaSoft.MvvmLight;

    public class ActivityTypeItem : ObservableObject
    {
        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set { Set(() => IsSelected, ref isSelected, value); }
        }

        public Type ActivityType { get; private set; }

        public Version Version { get; private set; }

        public ActivityTypeItem(Type activityType)
        {
            ActivityType = activityType;
            Version = activityType.Assembly.GetName().Version;
        }

        public override string ToString()
        {
            return ActivityType.Name;
        }
    }
}
