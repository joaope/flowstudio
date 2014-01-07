// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Build.Execution
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
    using System.Collections.Generic;

    public class VisualTrackingParticipant : TrackingParticipant
    {
        public event EventHandler<TrackingEventArgs> TrackingRecordReceived;

        public Dictionary<string, Activity> ActivityIdToWorkflowElementMap { get; set; }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            if (TrackingRecordReceived != null)
            {
                var activityStateRecord = record as ActivityStateRecord;

                if ((activityStateRecord != null) && (!activityStateRecord.Activity.TypeName.Contains("System.Activities.Expressions")))
                {
                    if (ActivityIdToWorkflowElementMap.ContainsKey(activityStateRecord.Activity.Id))
                    {
                        TrackingRecordReceived(
                            this,
                            new TrackingEventArgs(
                                record,
                                timeout,
                                ActivityIdToWorkflowElementMap[activityStateRecord.Activity.Id]));
                    }
                }
                else
                {
                    TrackingRecordReceived(this, new TrackingEventArgs(record, timeout, null));
                }
            }
        }
    }
}
