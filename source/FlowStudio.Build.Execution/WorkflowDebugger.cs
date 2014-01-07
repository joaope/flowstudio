// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Build.Execution
{
    using System;
    using System.Activities;
    using System.Activities.Debugger;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Debug;
    using System.Activities.Presentation.Services;
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Common.Services;
    using Properties;

    public abstract class WorkflowDebugger : IWorkflowDebugger
    {
        protected WorkflowDesigner WorkflowDesigner { get; private set; }

        protected TextWriter OutputWriter { get; private set; }

        protected bool IsRunningInternal { get; set; }

        protected WorkflowDebugger(WorkflowDesigner workflowDesigner, TextWriter outputWriter)
        {
            WorkflowDesigner = workflowDesigner;
            OutputWriter = outputWriter;
        }
        private readonly IDictionary<int, SourceLocation> stepSourceLocationMapping = new Dictionary<int, SourceLocation>();

        private uint stepCount;

        public bool IsRunning { get; private set; }

        protected void OnRunningStateChanged(WorkflowExecutingStateEventArgs e)
        {
            IsRunning = e.IsExecuting;

            if (ExecutingStateChanged != null)
            {
                ExecutingStateChanged(this, e);
            }
        }

        public void Start()
        {
            WorkflowDesigner.Flush();
            StartInternal();
        }

        protected abstract Activity GetDebuggableActivity(Activity root);

        protected abstract void StartInternal();

        protected abstract void StopInternal();

        public void Stop()
        {
            StopInternal();
        }

        public event EventHandler<WorkflowExecutingStateEventArgs> ExecutingStateChanged;

        public void HighlightActivity(int selectedRowNumber)
        {
            DispatcherService.Dispatch(() =>
            {
                try
                {
                    if (selectedRowNumber >= 0 && selectedRowNumber < stepSourceLocationMapping.Count)
                    {
                        WorkflowDesigner.DebugManagerView.CurrentLocation = stepSourceLocationMapping[selectedRowNumber];
                    }
                }
                catch (Exception)
                {
                    // If the user clicks other than on the tracking records themselves.
                    WorkflowDesigner.DebugManagerView.CurrentLocation = new SourceLocation("Workflow.xaml", 1, 1, 1, 10);
                }
            });
        }

        protected VisualTrackingParticipant InitialiseVisualTrackingParticipant(Activity activityToRun)
        {
            // Mapping between the object and Line No.
            var elementToSourceLocationMap = UpdateSourceLocationMappingInDebuggerService(activityToRun);

            // Mapping between the object and the Instance Id
            var activityIdToElementMap = elementToSourceLocationMap
                .Keys
                .OfType<Activity>()
                .ToDictionary(workflowElement => workflowElement.Id);

            // Setup custom tracking
            var participant = new VisualTrackingParticipant
            {
                ActivityIdToWorkflowElementMap = activityIdToElementMap,
                TrackingProfile = new TrackingProfile
                {
                    Name = "VisualTrackingProfile",
                    Queries =
                    {
                        new CustomTrackingQuery
                        {
                            Name = "*",
                            ActivityName = "*"
                        },
                        new WorkflowInstanceQuery
                        {
                            // Limit workflow instance tracking records for started and completed workflow states
                            States = {WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed},
                        },
                        new ActivityStateQuery
                        {
                            // Subscribe for track records from all activities for all states
                            ActivityName = "*",
                            States = { "*" },

                            // Extract workflow variables and arguments as a part of the activity tracking record
                            // VariableName = "*" allows for extraction of all variables in the scope
                            // of the activity
                            Variables = { "*" }
                        }
                    }
                }
            };

            var debugInterval = Settings.Default.MillisecondsBetweenDebugSteps > 0
                                    ? Settings.Default.MillisecondsBetweenDebugSteps
                                    : 1000;

            // As the tracking events are received
            participant.TrackingRecordReceived += (trackingParticipant, trackingEventArgs) =>
            {
                if (trackingEventArgs.Activity != null)
                {
                    DispatcherService.Dispatch(() =>
                    {
                        WorkflowDesigner.DebugManagerView.CurrentLocation =
                            elementToSourceLocationMap[trackingEventArgs.Activity];
                    });

                    Thread.Sleep(debugInterval);

                    var debugItem = new DebugStep(
                        stepCount++,
                        trackingEventArgs.Record.EventTime,
                        trackingEventArgs.Activity.DisplayName,
                        trackingEventArgs.Activity.Id,
                        trackingEventArgs.Record.InstanceId,
                        ((ActivityStateRecord) trackingEventArgs.Record).State);

                    stepSourceLocationMapping.Add((int)stepCount - 1, elementToSourceLocationMap[trackingEventArgs.Activity]);
                    OnDebugStepAdded(debugItem);
                }
            };

            return participant;
        }

        private Dictionary<object, SourceLocation> UpdateSourceLocationMappingInDebuggerService(Activity root)
        {
            var rootInstance = GetRootInstance();
            var sourceLocationMapping = new Dictionary<object, SourceLocation>();
            var designerSourceLocationMapping = new Dictionary<object, SourceLocation>();

            if (rootInstance != null)
            {
                var documentRootElement = GetRootWorkflowElement(rootInstance);

                SourceLocationProvider.CollectMapping(
                    GetDebuggableActivity(root),
                    documentRootElement,
                    sourceLocationMapping,
                    WorkflowDesigner.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);

                SourceLocationProvider.CollectMapping(
                   documentRootElement,
                   documentRootElement,
                   designerSourceLocationMapping,
                   WorkflowDesigner.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);
            }

            ((DebuggerService)WorkflowDesigner.DebugManagerView).UpdateSourceLocations(designerSourceLocationMapping);

            return sourceLocationMapping;
        }

        private object GetRootInstance()
        {
            var modelService = WorkflowDesigner.Context.Services.GetService<ModelService>();
            return modelService != null ? modelService.Root.GetCurrentValue() : null;
        }

        private Activity GetRootWorkflowElement(object rootModelObject)
        {
            Activity rootWorkflowElement;
            var debuggableWorkflowTree = rootModelObject as IDebuggableWorkflowTree;
            if (debuggableWorkflowTree != null)
            {
                rootWorkflowElement = debuggableWorkflowTree.GetWorkflowRoot();
            }
            else
            {
                rootWorkflowElement = rootModelObject as Activity;
            }

            return rootWorkflowElement;
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DebugStepEventArgs> DebugStepAdded;

        protected void OnDebugStepAdded(DebugStep debugStep)
        {
            if (DebugStepAdded != null)
            {
                DebugStepAdded(this, new DebugStepEventArgs(debugStep));
            }
        }
    }
}
