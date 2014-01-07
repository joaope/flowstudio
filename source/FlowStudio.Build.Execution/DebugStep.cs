// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Build.Execution
{
    using System;

    public class DebugStep
    {
        public DebugStep(uint stepCount, DateTime eventTime, string activityName, string activityId, Guid instanceId, string state)
        {
            StepCount = stepCount;
            EventTime = eventTime;
            ActivityName = activityName;
            ActivityId = activityId;
            InstanceId = instanceId;
            State = state;
        }

        public DateTime EventTime { get; set; }  

        public string ActivityId { get; private set; }

        public uint StepCount { get; private set; }

        public string ActivityName { get; private set; }

        public Guid InstanceId { get; private set; }

        public string State { get; private set; }
    }
}
