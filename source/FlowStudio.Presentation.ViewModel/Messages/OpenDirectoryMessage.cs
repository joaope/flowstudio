// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel.Messages
{
    using System;
    using GalaSoft.MvvmLight.Messaging;

    public class OpenDirectoryMessage : NotificationMessageAction<OpenDirectoryResult>
    {
        public OpenDirectoryMessage(
            string notification,
            Action<OpenDirectoryResult> callback)
            : base(notification, callback)
        {
        }

        public OpenDirectoryMessage(
            object sender,
            string notification,
            Action<OpenDirectoryResult> callback)
            : base(sender, notification, callback)
        {
        }

        public OpenDirectoryMessage(
            object sender,
            object target,
            string notification,
            Action<OpenDirectoryResult> callback)
            : base(sender, target, notification, callback)
        {
        }
    }
}
