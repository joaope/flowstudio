// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Messages
{
    using System;
    using GalaSoft.MvvmLight.Messaging;

    public class OpenFileMessage : NotificationMessageAction<OpenFileResult>
    {
        public bool Multiselect { get; private set; }

        public OpenFileMessage(
            bool multiselect,
            string notification,
            Action<OpenFileResult> callback)
            : base(notification, callback)
        {
            Multiselect = multiselect;
        }

        public OpenFileMessage(
            bool multiselect,
            object sender,
            string notification,
            Action<OpenFileResult> callback)
            : base(sender, notification, callback)
        {
            Multiselect = multiselect;
        }

        public OpenFileMessage(
            bool multiselect,
            object sender,
            object target,
            string notification,
            Action<OpenFileResult> callback)
            : base(sender, target, notification, callback)
        {
            Multiselect = multiselect;
        }
    }
}
