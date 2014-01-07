// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel
{
    public abstract class FilePaneViewModel : PaneViewModel
    {
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { Set(() => FilePath, ref filePath, value); }
        }

        protected FilePaneViewModel(string contentId)
            : base(contentId)
        {
        }
    }
}
