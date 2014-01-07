// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Messages
{
    using System;
    using GalaSoft.MvvmLight.Messaging;

    public class RequestCloseProgramMessage : NotificationMessageAction<bool>
    {
        public RequestCloseProgramMessage(string notification, Action<bool> callback)
            : base(notification, callback)
        {
        }

        public RequestCloseProgramMessage(object sender, string notification, Action<bool> callback)
            : base(sender, notification, callback)
        {
        }

        public RequestCloseProgramMessage(object sender, object target, string notification, Action<bool> callback)
            : base(sender, target, notification, callback)
        {
        }
    }
}
