// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Messages
{
    using System;
    using Data;
    using GalaSoft.MvvmLight.Messaging;

    public class SaveFileMessage : NotificationMessageAction<SaveFileResult>
    {
        public WorkflowFileInfo WorkflowFileInfo { get; private set; }

        public SaveFileMessage(WorkflowFileInfo workflowFileInfo, string notification, Action<SaveFileResult> callback)
            : base(notification, callback)
        {
            WorkflowFileInfo = workflowFileInfo;
        }

        public SaveFileMessage(WorkflowFileInfo workflowFileInfo, object sender, string notification, Action<SaveFileResult> callback)
            : base(sender, notification, callback)
        {
            WorkflowFileInfo = workflowFileInfo;
        }

        public SaveFileMessage(WorkflowFileInfo workflowFileInfo, object sender, object target, string notification, Action<SaveFileResult> callback)
            : base(sender, target, notification, callback)
        {
            WorkflowFileInfo = workflowFileInfo;
        }
    }
}
