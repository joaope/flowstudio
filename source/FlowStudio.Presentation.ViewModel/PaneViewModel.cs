// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel
{
    using Common;

    public abstract class PaneViewModel : ViewModel
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { Set(() => Title, ref title, value); }
        }

        private string contentId;

        public string ContentId
        {
            get { return contentId; }
            set { Set(() => ContentId, ref contentId, value); }
        }

        protected PaneViewModel(string contentId)
        {
            ContentId = contentId;
        }
    }
}
