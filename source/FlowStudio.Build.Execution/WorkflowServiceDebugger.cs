// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Build.Execution
{
    using System;
    using System.Activities;
    using System.Activities.Presentation;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.Text;
    using System.Xaml;

    public class WorkflowServiceDebugger : WorkflowDebugger
    {
        private WorkflowServiceHost workflowServiceHost;

        public WorkflowServiceDebugger(WorkflowDesigner workflowDesigner, TextWriter outputWriter) 
            : base(workflowDesigner, outputWriter)
        {
        }

        protected override Activity GetDebuggableActivity(Activity root)
        {
            WorkflowInspectionServices.CacheMetadata(root);
            return root;
        }

        protected override void StartInternal()
        {
            WorkflowService workflowService;
            
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(WorkflowDesigner.Text)))
            {
                workflowService = XamlServices.Load(stream) as WorkflowService;
            }

            workflowServiceHost = new WorkflowServiceHost(workflowService);

            workflowServiceHost.WorkflowExtensions.Add(OutputWriter);
            workflowServiceHost.WorkflowExtensions.Add(InitialiseVisualTrackingParticipant(workflowService.GetWorkflowRoot()));

            workflowServiceHost.Opened += WorkflowServiceHostOnOpened;
            workflowServiceHost.Opening += WorkflowServiceHostOnOpening;
            workflowServiceHost.Closed += WorkflowServiceHostOnClosed;
            workflowServiceHost.Closing += WorkflowServiceHostOnClosing;
            workflowServiceHost.Faulted += WorkflowServiceHostOnFaulted;
            workflowServiceHost.UnknownMessageReceived += WorkflowServiceHostOnUnknownMessageReceived;
        }

        private void WorkflowServiceHostOnUnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs unknownMessageReceivedEventArgs)
        {
            OutputWriter.WriteLine("Unknown workflow service message received: {0}", unknownMessageReceivedEventArgs.Message);
        }

        private void WorkflowServiceHostOnFaulted(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("Workflow service faulted for unknown reasons");
            OnRunningStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        private void WorkflowServiceHostOnClosing(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("Workflow Service closing...");
        }

        private void WorkflowServiceHostOnClosed(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("Workflow Service successfully closed.");
            OnRunningStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        private void WorkflowServiceHostOnOpening(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("Workflow Service opening...");
        }

        private void WorkflowServiceHostOnOpened(object sender, EventArgs eventArgs)
        {
            OutputWriter.WriteLine("Workflow Service opened.");
            OnRunningStateChanged(new WorkflowExecutingStateEventArgs(false));
        }

        protected override void StopInternal()
        {
            if (IsRunning && workflowServiceHost != null)
            {
                workflowServiceHost.Close();
            }
        }
    }
}
