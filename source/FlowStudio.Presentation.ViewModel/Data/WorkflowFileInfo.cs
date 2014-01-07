// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Data
{
    public class WorkflowFileInfo
    {
        public string Title { get; private set; }

        public bool IsDirty { get; private set; }

        public WorkflowType WorkflowType { get; private set; }

        public string FilePath { get; private set; }

        public WorkflowFileInfo(string title, bool isDirty, WorkflowType workflowType, string filePath)
        {
            Title = title;
            IsDirty = isDirty;
            WorkflowType = workflowType;
            FilePath = filePath;
        }

        public WorkflowFileInfo(string title, bool isDirty, WorkflowType workflowType)
            : this(title, isDirty, workflowType, null)
        {
        }

        public WorkflowFileInfo(WorkflowFileViewModel fileModel)
            : this(fileModel.Title, fileModel.IsDirty, fileModel.WorkflowType, fileModel.FilePath)
        {
        }
    }
}
