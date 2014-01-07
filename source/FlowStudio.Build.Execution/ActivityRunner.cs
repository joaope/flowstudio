// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Build.Execution
{
    using System;
    using System.Activities;
    using System.Activities.XamlIntegration;
    using System.IO;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ActivityRunner : WorkflowRunner
    {
        private WorkflowApplication workflowApplication;

        public ActivityRunner(string xaml, TextWriter outputWriter) 
            : base(xaml, outputWriter)
        {
        }

        public override void Start()
        {
            if (IsRunning)
            {
                return;
            }

            using (var stream = new MemoryStream(Encoding.Default.GetBytes(Xaml)))
            {
                workflowApplication = new WorkflowApplication(ActivityXamlServices.Load(stream));
            }

            workflowApplication.Extensions.Add(OutputWriter);

            workflowApplication.Completed += OnWorkflowCompleted;
            workflowApplication.Aborted += OnWorkflowAborted;
            workflowApplication.OnUnhandledException += OnWorkflowUnhandledException;

            try
            {
                OnExecutingStateChanged(new WorkflowExecutingStateEventArgs(true));
                workflowApplication.Run();
            }
            catch (Exception e)
            {
                OutputWriter.WriteLine(e.StackTrace);
                OnExecutingStateChanged(new WorkflowExecutingStateEventArgs(false));
            }
        }

        private UnhandledExceptionAction OnWorkflowUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            OutputWriter.WriteLine(e.UnhandledException.StackTrace);
            return UnhandledExceptionAction.Terminate;
        }

        private void OnWorkflowAborted(WorkflowApplicationAbortedEventArgs e)
        {
            OnExecutingStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        private void OnWorkflowCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            OnExecutingStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        public override void Stop()
        {
            if (IsRunning && workflowApplication != null)
            {
                workflowApplication.Abort();
            }
        }
    }
}
