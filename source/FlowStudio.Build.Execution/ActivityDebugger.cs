// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Build.Execution
{
    using System;
    using System.Activities;
    using System.Activities.Presentation;
    using System.Activities.XamlIntegration;
    using System.IO;
    using System.Text;

    public class ActivityDebugger : WorkflowDebugger
    {
        private WorkflowApplication workflowApplication;

        public ActivityDebugger(WorkflowDesigner workflowDesigner, TextWriter outputWriter) 
            : base(workflowDesigner, outputWriter)
        {
        }

        protected override Activity GetDebuggableActivity(Activity root)
        {
            WorkflowInspectionServices.CacheMetadata(root);

            var enumerator = WorkflowInspectionServices.GetActivities(root).GetEnumerator();

            enumerator.MoveNext();
            return enumerator.Current;
        }

        protected override void StartInternal()
        {
            DynamicActivity activity;

            using (var stream = new MemoryStream(Encoding.Default.GetBytes(WorkflowDesigner.Text)))
            {
                activity = ActivityXamlServices.Load(stream) as DynamicActivity;
            }

            if (activity == null)
            {
                return;
            }

            WorkflowInspectionServices.CacheMetadata(activity);

            workflowApplication = new WorkflowApplication(activity);

            workflowApplication.Extensions.Add(OutputWriter);
            workflowApplication.Extensions.Add(InitialiseVisualTrackingParticipant(activity));

            workflowApplication.Completed += Completed;
            workflowApplication.OnUnhandledException += OnUnhandledException;
            workflowApplication.Aborted += Aborted;

            try
            {
                workflowApplication.Run();
                OnRunningStateChanged(new WorkflowExecutingStateEventArgs(true));
            }
            catch (Exception e)
            {
                OutputWriter.WriteLine(e.StackTrace);
            }
        }

        private void Aborted(WorkflowApplicationAbortedEventArgs e)
        {
            OnRunningStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        private UnhandledExceptionAction OnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            OutputWriter.WriteLine(e.UnhandledException.StackTrace);
            OnRunningStateChanged(new WorkflowExecutingStateEventArgs(false));

            return UnhandledExceptionAction.Terminate;
        }

        private void Completed(WorkflowApplicationCompletedEventArgs e)
        {
            OnRunningStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        protected override void StopInternal()
        {
            if (IsRunning && workflowApplication != null)
            {
                workflowApplication.Abort();
            }
        }
    }
}
