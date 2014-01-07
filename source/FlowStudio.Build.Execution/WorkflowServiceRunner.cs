// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Build.Execution
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.Text;
    using System.Xaml;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WorkflowServiceRunner : WorkflowRunner
    {
        private WorkflowServiceHost workflowServiceHost;

        public WorkflowServiceRunner(string xaml, TextWriter outputWriter) 
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
                workflowServiceHost = new WorkflowServiceHost(XamlServices.Load(stream) as WorkflowService);
            }

            workflowServiceHost.WorkflowExtensions.Add(OutputWriter);

            workflowServiceHost.Opened += WorkflowServiceHostOnOpened;
            workflowServiceHost.Opening += WorkflowServiceHostOnOpening;
            workflowServiceHost.Closed += WorkflowServiceHostOnClosed;
            workflowServiceHost.Closing += WorkflowServiceHostOnClosing;
            workflowServiceHost.Faulted += WorkflowServiceHostOnFaulted;
            workflowServiceHost.UnknownMessageReceived += WorkflowServiceHostOnUnknownMessageReceived;

            try
            {
                OnExecutingStateChanged(new WorkflowExecutingStateEventArgs(true));
                workflowServiceHost.Open();
            }
            catch (Exception e)
            {
                OutputWriter.WriteLine(e.StackTrace);
                OnExecutingStateChanged(new WorkflowExecutingStateEventArgs(false));
            }
        }

        private void WorkflowServiceHostOnUnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs unknownMessageReceivedEventArgs)
        {
            OutputWriter.WriteLine("Unknown workflow service message received: {0}", unknownMessageReceivedEventArgs.Message);
        }

        private void WorkflowServiceHostOnFaulted(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("ERROR: workflow service faulted for unknown resons");
            OnExecutingStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        private void WorkflowServiceHostOnClosing(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("Workflow Service closing...");
        }

        private void WorkflowServiceHostOnClosed(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("Workflow Service successfully closed.");
            OnExecutingStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        private void WorkflowServiceHostOnOpening(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("Workflow Service opening...");
        }

        private void WorkflowServiceHostOnOpened(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("Workflow Service opened.");
            OnExecutingStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        public override void Stop()
        {
            if (IsRunning && workflowServiceHost != null)
            {
                workflowServiceHost.Close();
            }
        }
    }
}
