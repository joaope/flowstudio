// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel
{
    public abstract class ToolViewModel : PaneViewModel
    {
        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set { Set(() => IsVisible, ref isVisible, value); }
        }

        protected ToolViewModel(string contentId, string title)
            : base(contentId)
        {
            Title = title;
        }
    }
}
