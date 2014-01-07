// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel
{
    using System;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Services;
    using System.Activities.Presentation.Validation;
    using System.Activities.Presentation.View;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel.Activities;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Build.Execution;
    using Data;
    using GalaSoft.MvvmLight.Command;
    using Messages;
    using System.Activities.Core.Presentation;

    public class WorkflowFileViewModel : FilePaneViewModel, IValidationErrorService
    {
        private WorkflowDesigner workflowDesigner;

        public WorkflowDesigner WorkflowDesigner
        {
            get { return workflowDesigner; }
            set
            {
                workflowDesigner = value;

                WorkflowDesigner.Context.Services.Publish<IValidationErrorService>(this);
                WorkflowDesigner.ModelChanged += workflowDesigner_ModelChanged;

                DesignerView = WorkflowDesigner.Context.Services.GetService<DesignerView>();
                undoEngine = WorkflowDesigner.Context.Services.GetService<UndoEngine>();

                RaisePropertyChanged("PropertyInspectorView");
                RaisePropertyChanged("WorkflowDesignerView");
            }
        }

        public UIElement WorkflowDesignerView
        {
            get { return workflowDesigner.View; }
        }

        public UIElement PropertyInspectorView
        {
            get { return workflowDesigner.PropertyInspectorView; }
        }

        private UndoEngine undoEngine;

        private DesignerView designerView;

        public DesignerView DesignerView
        {
            get { return designerView; }
            private set { Set(() => DesignerView, ref designerView, value); }
        }

        private readonly ObservableCollection<ValidationErrorInfo> validationErrorInfos = new ObservableCollection<ValidationErrorInfo>(); 

        public ObservableCollection<ValidationErrorInfo> ValidationErrorInfos
        {
            get { return validationErrorInfos; }
        }

        private readonly ObservableCollection<DebugStep> debugSteps = new ObservableCollection<DebugStep>();

        public ObservableCollection<DebugStep> DebugSteps
        {
            get { return debugSteps; }
        }

        public bool IsSaved { get; private set; }

        public bool IsDirty { get; private set; }

        public WorkflowType WorkflowType { get; private set; }

        private IWorkflowRunner executer;

        private bool isExecuting;

        public bool IsExecuting
        {
            get { return isExecuting; }
            private set { Set(() => IsExecuting, ref isExecuting, value); }
        }

        private readonly StringBuilder outputTextBuilder = new StringBuilder();

        private EventableStringWriter outputTextWriter;

        public string OutputText
        {
            get { return outputTextBuilder.ToString(); }
        }

        private void InitializeWorkflowFileViewModel()
        {
            new DesignerMetadata().Register();
            WorkflowDesigner = new WorkflowDesigner();

            outputTextWriter = new EventableStringWriter(outputTextBuilder);
            outputTextWriter.TextChanged += (sender, args) => RaisePropertyChanged("OutputText");

            Outline = new OutlineItemObservableCollection();
        }

        private void FinalizeWorkflowFileViewModel()
        {
            UpdateOutlineInformation();
        }

        public WorkflowFileViewModel(WorkflowType workflowType, uint dirtyCounter)
            : base(string.Concat(workflowType, dirtyCounter))
        {
            InitializeWorkflowFileViewModel();

            WorkflowType = workflowType;
            IsDirty = true;
            IsSaved = true;

            Title = string.Format(
                "{0} {1}",
                workflowType == WorkflowType.Activity ? "Activity" : "WorkflowService",
                dirtyCounter);

            FilePath = string.Concat(
                Title,
                workflowType == WorkflowType.Activity ? ".xaml" : ".xamlx");

            var resourcePath = workflowType == WorkflowType.Activity
                ? "FlowStudio.DefaultActivity.xaml"
                : "FlowStudio.DefaultWorkflowService.xamlx";

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        WorkflowDesigner.Text = reader.ReadToEnd();
                    }
                }
            }

            WorkflowDesigner.Load();

            FinalizeWorkflowFileViewModel();
        }

        public WorkflowFileViewModel(string filePath)
            : base(filePath)
        {
            InitializeWorkflowFileViewModel();

            IsSaved = true;
            IsDirty = false;
            FilePath = filePath;
            Title = Path.GetFileName(filePath);

            WorkflowDesigner.Load(filePath);

            WorkflowType =
                WorkflowDesigner.Context.Services.GetService<ModelService>().Root.GetCurrentValue() is WorkflowService
                    ? WorkflowType.WorkflowService
                    : WorkflowType.Activity;

            FinalizeWorkflowFileViewModel();
        }

        private void workflowDesigner_ModelChanged(object sender, EventArgs e)
        {
            UpdateOutlineInformation();

            if (IsSaved)
            {
                IsSaved = false;
                Title = string.Format("{0} *", Title);
            }
        }

        private OutlineItemObservableCollection outline;

        public OutlineItemObservableCollection Outline
        {
            get { return outline; }
            set { Set(() => Outline, ref outline, value); }
        }

        private void UpdateOutlineInformation()
        {
            var worker = new BackgroundWorker();

            worker.DoWork += (sender,
                              args) =>
                {
                    args.Result = OutlineItemFactory.GetOutlineItem(
                        WorkflowDesigner.Context.Services.GetService<ModelService>().Root);
                };

            worker.RunWorkerCompleted += (sender,
                                          args) =>
                {
                    Outline.Clear();
                    Outline.Add((OutlineItem)args.Result);
                };

            worker.RunWorkerAsync();
        }

        public event EventHandler<WorkflowFileCloseEventArgs> CloseWorkflowFileClicked;

        public event EventHandler<WorkflowFileCloseEventArgs> CloseAllButThisWorkflowFileClicked; 

        public void ShowValidationErrors(IList<ValidationErrorInfo> errors)
        {
            ValidationErrorInfos.Clear();

            foreach (var validationErrorInfo in errors)
            {
                ValidationErrorInfos.Add(validationErrorInfo);
            }
        }

        public void Run()
        {
            if (IsExecuting)
            {
                return;
            }

            WorkflowDesigner.Flush();

            if (WorkflowType == WorkflowType.Activity)
            {
                executer = new ActivityRunner(WorkflowDesigner.Text, outputTextWriter);
            }
            else
            {
                executer = new WorkflowServiceRunner(WorkflowDesigner.Text, outputTextWriter);
            }

            executer.ExecutingStateChanged += OnExecuterRunningStateChanged;
            executer.Start();
        }

        public void Debug()
        {
            if (IsExecuting || !Save(true))
            {
                return;
            }

            // HACK ZONE

            // WorkflowDesigner have to be loaded all over again through WorkflowDesigner.Load(filepath) otherwise
            // SourceLocationProvider.CollectMapping() won't collect nothing later on WorkflowDebugger and
            // becaquse of that debug process won't work as expected. 
            
            // Activities not loaded with WorkflowDesigner.Load(filepath) won't be recognized at all.

            // That's the only motive for the workflow to be saved before debug.

            // More info: http://social.msdn.microsoft.com/forums/en-us/wfprerelease/thread/2DEE4DD9-D3F7-4430-BCC7-D613859074D1
            //            http://social.msdn.microsoft.com/Forums/en/wfprerelease/thread/0a28cade-bd8d-417b-a1e4-efbc0d9ccd43
            //            http://blogs.msdn.com/b/tilovell/archive/2011/06/08/wf4-visual-workflow-tracking-and-workflowinspectionservices.aspx

            // This needs to be addressed by MS. Didn't test it on NET4.5
            // Should check this in detail later

            WorkflowDesigner = new WorkflowDesigner();
            WorkflowDesigner.Load(FilePath);

            // END OF HACK ZONE

            if (WorkflowType == WorkflowType.Activity)
            {
                executer = new ActivityDebugger(WorkflowDesigner, outputTextWriter);
            }
            else
            {
                executer = new WorkflowServiceDebugger(WorkflowDesigner, outputTextWriter);
            }

            ((IWorkflowDebugger) executer).DebugStepAdded += (sender, args) =>
                {
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (Action) (() => DebugSteps.Add(args.DebugStep)));
                };

            executer.ExecutingStateChanged += OnExecuterRunningStateChanged;
            executer.Start();
        }

        private void OnExecuterRunningStateChanged(object sender, WorkflowExecutingStateEventArgs e)
        {
            IsExecuting = e.IsExecuting;

            if (e.IsExecuting)
            {
                DebugSteps.Clear();
            }
        }

        public void Abort()
        {
            if (executer != null && executer.IsRunning)
            {
                executer.Stop();
            }
        }

        public bool Save()
        {
            return Save(false);
        }

        public bool Save(bool forceIfdirty)
        {
            if (IsSaved && !forceIfdirty)
            {
                return true;
            }

            var file = FilePath;

            if (IsDirty)
            {
                var saveResult = SaveFileResult.DoNothing;

                var saveMsg = new SaveFileMessage(
                    new WorkflowFileInfo(this), 
                    this,
                    null,
                    result =>
                    {
                        saveResult = result;
                    });

                MessengerInstance.Send(saveMsg);

                if (!saveResult.PerformSave)
                {
                    return false;
                }

                file = saveResult.FileName;
            }

            SaveWorkflow(file);
            return true;
        }

        public bool SaveAs()
        {
            bool finalResult = false;

            var saveMsg = new SaveFileMessage(
                new WorkflowFileInfo(this), 
                this,
                null,
                result =>
                    {
                        if (result.PerformSave)
                        {
                            SaveWorkflow(result.FileName);
                            finalResult = true;
                        }
                        else
                        {
                            finalResult = false;
                        }
                    });

            MessengerInstance.Send(saveMsg);

            return finalResult;
        }

        private void SaveWorkflow(string filePath)
        {
            WorkflowDesigner.Save(filePath);

            FilePath = filePath;
            IsSaved = true;
            IsDirty = false;
            Title = Path.GetFileName(filePath);
        }

        public IEnumerable<string> UndoActions
        {
            get { return undoEngine.GetUndoActions(); }
        }

        public IEnumerable<string> RedoActions
        {
            get { return undoEngine.GetRedoActions(); }
        }

        public void Undo()
        {
            undoEngine.Undo();
        }

        public void Redo()
        {
            undoEngine.Redo();
        }

        #region Commands

        private ICommand closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                {
                    closeCommand = new RelayCommand(
                        () =>
                        {
                            if (CloseWorkflowFileClicked != null)
                            {
                                CloseWorkflowFileClicked(this, new WorkflowFileCloseEventArgs(this));
                            }
                        });
                }

                return closeCommand;
            }
        }

        private ICommand closeAllButThisCommand;

        public ICommand CloseAllButThisCommand
        {
            get
            {
                if (closeAllButThisCommand == null)
                {
                    closeAllButThisCommand = new RelayCommand(
                        () =>
                        {
                            if (CloseAllButThisWorkflowFileClicked != null)
                            {
                                CloseAllButThisWorkflowFileClicked(this, new WorkflowFileCloseEventArgs(this));
                            }
                        });
                }

                return closeAllButThisCommand;
            }
        }

        #endregion
    }
}
