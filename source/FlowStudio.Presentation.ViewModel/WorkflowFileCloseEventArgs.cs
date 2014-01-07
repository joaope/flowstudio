// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel
{
    using System;

    public class WorkflowFileCloseEventArgs : EventArgs
    {
        public WorkflowFileViewModel WorkflowFile { get; private set; }

        public WorkflowFileCloseEventArgs(WorkflowFileViewModel workflowFile)
        {
            WorkflowFile = workflowFile;
        }
    }
}
