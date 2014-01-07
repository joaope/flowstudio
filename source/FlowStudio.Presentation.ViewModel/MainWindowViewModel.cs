// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel
{
    using System.Activities.Presentation.View;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using Data;
    using GalaSoft.MvvmLight.Command;
    using Messages;
    using Services;
    using Common;

    public class MainWindowViewModel : ViewModel
    {
        private readonly SynchronizationContext syncCtx;

        public MainWindowViewModel()
        {
            ToolboxViewModel = new ToolboxViewModel();
            ErrorsListViewModel = new ErrorsListViewModel();
            PropertyInspectorViewModel = new PropertyInspectorViewModel();
            DebugViewModel = new DebugViewModel();
            OutputViewModel = new OutputViewModel();
            WorkflowOutlineViewModel = new WorkflowOutlineViewModel();

            StatusViewModel.SetText("Ready");
            syncCtx = SynchronizationContext.Current;

            ToolboxViewModel.IsVisible = true;
            PropertyInspectorViewModel.IsVisible = true;
            OutputViewModel.IsVisible = true;
            ErrorsListViewModel.IsVisible = true;
            DebugViewModel.IsVisible = true;
            WorkflowOutlineViewModel.IsVisible = true;

            MessengerInstance.Register<RequestCloseProgramMessage>(this, OnRequestCloseProgramMessage);
        }

        private void OnRequestCloseProgramMessage(RequestCloseProgramMessage closeMessage)
        {
            if (files.Any(f => f.IsExecuting))
            {
                MessengerInstance.Send(new ClosingWhenExecutingMessage());
                closeMessage.Execute(false);
                return;
            }

            var notSavedFiles = Files
                .Where(f => !f.IsSaved)
                .ToList();

            if (notSavedFiles.Count == 0)
            {
                closeMessage.Execute(true);
                return;
            }

            MessengerInstance.Send(new SaveBeforeCloseMessage(
                notSavedFiles.Select(f => new WorkflowFileInfo(f)).ToList(),
                null,
                result =>
                {
                    if (result == QuestionResult.Cancel)
                    {
                        closeMessage.Execute(false);
                    }
                    else if (result == QuestionResult.Yes)
                    {
                        foreach (var notSavedFile in notSavedFiles)
                        {
                            var r = notSavedFile.Save();

                            if (!r)
                            {
                                closeMessage.Execute(false);
                                return;
                            }
                        }
                    }

                    closeMessage.Execute(true);
                }));
        }

        private uint dirtyCounter;

        private readonly ObservableCollection<WorkflowFileViewModel> files = new ObservableCollection<WorkflowFileViewModel>();

        private ReadOnlyObservableCollection<WorkflowFileViewModel> readonlyFiles; 

        public ReadOnlyObservableCollection<WorkflowFileViewModel> Files
        {
            get
            {
                return readonlyFiles
                       ?? (readonlyFiles = new ReadOnlyObservableCollection<WorkflowFileViewModel>(files));
            }
        }

        private string windowTitle = "Flow Studio";

        public string WindowTitle
        {
            get { return windowTitle; }
            set { Set(() => WindowTitle, ref windowTitle, value); }
        }

        public StatusViewModel StatusViewModel
        {
            get { return StatusViewModel.Instance; }
        }

        private ToolViewModel[] tools;

        public ToolViewModel[] Tools
        {
            get
            {
                if (tools == null)
                {
                    tools = new ToolViewModel[]
                    {
                        ToolboxViewModel,
                        ErrorsListViewModel,
                        PropertyInspectorViewModel,
                        OutputViewModel,
                        DebugViewModel,
                        WorkflowOutlineViewModel
                    };
                }

                return tools;
            }
        }

        public ToolboxViewModel ToolboxViewModel { get; private set; }

        public ErrorsListViewModel ErrorsListViewModel { get; private set; }

        public PropertyInspectorViewModel PropertyInspectorViewModel { get; private set; }

        public OutputViewModel OutputViewModel { get; private set; }

        public DebugViewModel DebugViewModel { get; private set; }

        public WorkflowOutlineViewModel WorkflowOutlineViewModel { get; private set; }

        public bool HasActiveWorkflowFile
        {
            get { return ActiveWorkflowFile != null; }
        }

        private WorkflowFileViewModel activeWorkflowFile;

        public WorkflowFileViewModel ActiveWorkflowFile
        {
            get { return activeWorkflowFile; }
            set
            {
                if (activeWorkflowFile != value)
                {
                    DispatcherService.Dispatch(() =>
                    {
                        activeWorkflowFile = value;

                        RaisePropertyChanged("ActiveWorkflowFile");
                        RaisePropertyChanged("CurrentDesignerView");
                        RaisePropertyChanged("HasActiveWorkflowFile");

                        UpdateWindowTitle();
                    });
                }
            }
        }

        private void file_CloseWorkflowFileClicked(object sender, WorkflowFileCloseEventArgs e)
        {
            var index = files.IndexOf(e.WorkflowFile);

            if (index == -1)
            {
                return;
            }

            var fileToClose = files[index];

            if (!fileToClose.IsSaved)
            {
                var saveResult = QuestionResult.Yes;

                var saveMessage = new SaveBeforeCloseMessage(
                    new List<WorkflowFileInfo> { new WorkflowFileInfo(fileToClose) }, 
                    null,
                    result =>
                        {
                            saveResult = result;
                        });

                MessengerInstance.Send(saveMessage);

                if (saveResult == QuestionResult.Cancel)
                {
                    return;
                }

                if (saveResult == QuestionResult.Yes)
                {
                    if (!fileToClose.Save())
                    {
                        return;
                    }
                }
            }

            files.Remove(fileToClose);

            if (files.Count == 0)
            {
                ActiveWorkflowFile = null;
            }
        }

        private void file_CloseAllButThisWorkflowFileClicked(object sender, WorkflowFileCloseEventArgs e)
        {
            var workflowsInfos = 
                files
                .Where(f => f != e.WorkflowFile && !f.IsSaved)
                .Select(f => new WorkflowFileInfo(f))
                .ToArray();

            var saveMessage = new SaveBeforeCloseMessage(
                workflowsInfos,
                null,
                result =>
                {
                    if (result == QuestionResult.Cancel)
                    {
                        return;
                    }

                    if (result == QuestionResult.Yes)
                    {
                        //file.Save();
                    }

                    //files.Remove(file);
                });

            MessengerInstance.Send(saveMessage);
        }

        private void CreateNewWorkfow(WorkflowType workflowType)
        {
            var file = new WorkflowFileViewModel(workflowType, ++dirtyCounter);
            files.Add(file);
            ActiveWorkflowFile = file;
            SubscribeToFileEvents(file);
        }

        private void SubscribeToFileEvents(WorkflowFileViewModel file)
        {
            file.CloseWorkflowFileClicked += file_CloseWorkflowFileClicked;
            file.CloseAllButThisWorkflowFileClicked += file_CloseAllButThisWorkflowFileClicked;

            file.PropertyChanged += file_PropertyChanged;
        }

        private void file_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Title")
            {
                UpdateWindowTitle();
            }
            else if (e.PropertyName == "IsExecuting")
            {
                syncCtx.Post(delegate { CommandManager.InvalidateRequerySuggested(); }, null);
            }
        }

        private void UpdateWindowTitle()
        {
            WindowTitle = ActiveWorkflowFile != null
                              ? string.Format("{0} - Flow Studio", ActiveWorkflowFile.Title)
                              : "Flow Studio";
        }

        #region Commands

        private bool preventNextNew;

        private ICommand newCommand;

        public ICommand NewCommand
        {
            get
            {
                if (newCommand == null)
                {
                    newCommand = new RelayCommand(
                        () =>
                        {
                            if (!preventNextNew)
                            {
                                CreateNewWorkfow(WorkflowType.Activity);
                                preventNextNew = true;
                            }
                            else
                            {
                                preventNextNew = false;
                            }
                        });
                }

                return newCommand;
            }
        }

        private ICommand newActivityCommand;

        public ICommand NewActivityCommand
        {
            get
            {
                if (newActivityCommand == null)
                {
                    newActivityCommand = new RelayCommand(
                        () => CreateNewWorkfow(WorkflowType.Activity));
                }

                return newActivityCommand;
            }
        }

        private ICommand newWorkflowServiceCommand;

        public ICommand NewWorkflowServiceCommand
        {
            get
            {
                if (newWorkflowServiceCommand == null)
                {
                    newWorkflowServiceCommand = new RelayCommand(
                        () => CreateNewWorkfow(WorkflowType.WorkflowService));
                }

                return newWorkflowServiceCommand;
            }
        }

        private ICommand openAboutCommand;

        public ICommand OpenAboutCommand
        {
            get
            {
                if (openAboutCommand == null)
                {
                    openAboutCommand = new RelayCommand(
                        () => MessengerInstance.Send(new OpenAboutDialogMessage()));
                }

                return openAboutCommand;
            }
        }

        private ICommand openOptionsCommand;

        public ICommand OpenOptionsCommand
        {
            get
            {
                if (openOptionsCommand == null)
                {
                    openOptionsCommand = new RelayCommand(
                        () => MessengerInstance.Send(new OpenOptionsMessage()));
                }

                return openOptionsCommand;
            }
        }

        private ICommand openCommand;

        public ICommand OpenCommand
        {
            get
            {
                if (openCommand == null)
                {
                    openCommand = new RelayCommand(OnOpenCommandInternal);
                }

                return openCommand;
            }
        }

        private void OnOpenCommandInternal()
        {
            var openMessage = new OpenFileMessage(
                true,
                this,
                null,
                result =>
                    {
                        if (result.PerformOpen)
                        {
                            var possibleNextActive = ActiveWorkflowFile;

                            foreach (var fileName in result.FileNames.Where(f => f != null))
                            {
                                if (!files.Any(f => f.FilePath == fileName))
                                {
                                    var newFile = new WorkflowFileViewModel(fileName);
                                    files.Add(newFile);
                                    SubscribeToFileEvents(newFile);

                                    possibleNextActive = newFile;
                                }
                                else
                                {
                                    possibleNextActive = files.SingleOrDefault(f => f.FilePath == fileName);
                                }
                            }

                            if (possibleNextActive != null)
                            {
                                ActiveWorkflowFile = possibleNextActive;
                            }
                        }
                    });

            MessengerInstance.Send(openMessage);
        }

        private ICommand saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(
                        () => activeWorkflowFile.Save(),
                        () => activeWorkflowFile != null && !activeWorkflowFile.IsSaved);
                }

                return saveCommand;
            }
        }

        private ICommand saveAsCommand;

        public ICommand SaveAsCommand
        {
            get
            {
                if (saveAsCommand == null)
                {
                    saveAsCommand = new RelayCommand(
                        () => activeWorkflowFile.SaveAs(),
                        () => ActiveWorkflowFile != null);
                }

                return saveAsCommand;
            }
        }

        private ICommand runWithDebuggingCommand;

        public ICommand RunWithDebuggingCommand
        {
            get
            {
                if (runWithDebuggingCommand == null)
                {
                    runWithDebuggingCommand = new RelayCommand(
                        () => activeWorkflowFile.Debug(),
                        () => ActiveWorkflowFile != null && !ActiveWorkflowFile.IsExecuting);
                }

                return runWithDebuggingCommand;
            }
        }

        private ICommand runWithoutDebuggingCommand;

        public ICommand RunWithoutDebuggingCommand
        {
            get
            {
                if (runWithoutDebuggingCommand == null)
                {
                    runWithoutDebuggingCommand = new RelayCommand(
                        () => activeWorkflowFile.Run(),
                        () => ActiveWorkflowFile != null && !ActiveWorkflowFile.IsExecuting);
                }

                return runWithoutDebuggingCommand;
            }
        }

        private ICommand abortExecutionCommand;

        public ICommand AbortExecutionCommand
        {
            get
            {
                if (abortExecutionCommand == null)
                {
                    abortExecutionCommand = new RelayCommand(
                        () => ActiveWorkflowFile.Abort(),
                        () => ActiveWorkflowFile != null && ActiveWorkflowFile.IsExecuting);
                }

                return abortExecutionCommand;
            }
        }

        private ICommand undoCommand;

        public ICommand UndoCommand
        {
            get
            {
                if (undoCommand == null)
                {
                    undoCommand = new RelayCommand(
                        () => ActiveWorkflowFile.Undo(),
                        () => ActiveWorkflowFile != null && ActiveWorkflowFile.UndoActions.Any());
                }

                return undoCommand;
            }
        }

        private ICommand redoCommand;

        public ICommand RedoCommand
        {
            get
            {
                if (redoCommand == null)
                {
                    redoCommand = new RelayCommand(
                        () => ActiveWorkflowFile.Redo(),
                        () => ActiveWorkflowFile != null && ActiveWorkflowFile.RedoActions.Any());
                }

                return redoCommand;
            }
        }

        private ICommand addReferenceCommand;

        public ICommand AddReferenceCommand
        {
            get
            {
                if (addReferenceCommand == null)
                {
                    addReferenceCommand = new RelayCommand(
                        () => ToolboxViewModel.AddReference());
                }

                return addReferenceCommand;
            }
        }

        public ICommand CutCommand
        {
            get { return DesignerView.CutCommand; }
        }

        public ICommand CopyCommand
        {
            get { return DesignerView.CopyCommand; }
        }

        public ICommand PasteCommand
        {
            get { return DesignerView.PasteCommand; }
        }

        public ICommand ZoomInCommand
        {
            get { return DesignerView.ZoomInCommand; }
        }

        public ICommand ZoomOutCommand
        {
            get { return DesignerView.ZoomOutCommand; }
        }

        public ICommand FitToScreenCommand
        {
            get { return DesignerView.FitToScreenCommand; }
        }

        public ICommand ResetZoomCommand
        {
            get { return DesignerView.ResetZoomCommand; }
        }

        public ICommand MoveFocusCommand
        {
            get { return DesignerView.MoveFocusCommand; }
        }

        public ICommand CollapseCommand
        {
            get { return DesignerView.CollapseCommand; }
        }

        public ICommand ExpandCommand
        {
            get { return DesignerView.ExpandCommand; }
        }

        public ICommand ExpandAllCommand
        {
            get { return DesignerView.ExpandAllCommand; }
        }

        public ICommand ExpandInPlaceCommand
        {
            get { return DesignerView.ExpandInPlaceCommand; }
        }

        public ICommand GoToParentCommand
        {
            get { return DesignerView.GoToParentCommand; }
        }

        public ICommand CreateArgumentCommand
        {
            get { return DesignerView.CreateArgumentCommand; }
        }

        public ICommand CreateVariableCommand
        {
            get { return DesignerView.CreateVariableCommand; }
        }

        public ICommand CreateWorkflowElementCommand
        {
            get { return DesignerView.CreateWorkflowElementCommand; }
        }

        public ICommand SaveAsImageCommand
        {
            get { return DesignerView.SaveAsImageCommand; }
        }

        public ICommand CopyAsImageCommand
        {
            get { return DesignerView.CopyAsImageCommand; }
        }

        #endregion
    }
}
