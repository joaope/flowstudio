// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Build.Execution
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;

    public class TrackingEventArgs : EventArgs
    {
        public TrackingEventArgs(TrackingRecord trackingRecord, TimeSpan timeout, Activity activity)
        {
            Record = trackingRecord;
            Timeout = timeout;
            Activity = activity;
        }

        public TrackingRecord Record { get; set; }

        public TimeSpan Timeout { get; set; }

        public Activity Activity { get; set; }
    }
}
