// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Messages
{
    public class OpenDirectoryResult
    {
        public bool PerformOpen { get; private set; }

        public string DirectoryPath { get; private set; }

        private OpenDirectoryResult()
        {
        }

        public OpenDirectoryResult(string directoryPath)
        {
            PerformOpen = true;
            DirectoryPath = directoryPath;
        }

        public static OpenDirectoryResult DoNothing
        {
            get
            {
                return new OpenDirectoryResult();
            }
        }
    }
}
