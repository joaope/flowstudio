// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Messages
{
    using System;
    using System.Collections.Generic;
    using Data;
    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SaveBeforeCloseMessage : NotificationMessageAction<QuestionResult>
    {
        public IList<WorkflowFileInfo> WorkflowsInfos { get; private set; }

        public SaveBeforeCloseMessage(IList<WorkflowFileInfo>  workflowsInfos, string notification, Action<QuestionResult> callback)
            : base(notification, callback)
        {
            WorkflowsInfos = workflowsInfos;
        }

        public SaveBeforeCloseMessage(IList<WorkflowFileInfo> workflowsInfos, object sender, string notification, Action<QuestionResult> callback)
            : base(sender, notification, callback)
        {
            WorkflowsInfos = workflowsInfos;
        }

        public SaveBeforeCloseMessage(IList<WorkflowFileInfo> workflowsInfos, object sender, object target, string notification, Action<QuestionResult> callback)
            : base(sender, target, notification, callback)
        {
            WorkflowsInfos = workflowsInfos;
        }
    }
}
