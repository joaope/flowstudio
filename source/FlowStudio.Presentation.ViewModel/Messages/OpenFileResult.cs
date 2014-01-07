// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Messages
{
    public class OpenFileResult
    {
        public bool PerformOpen { get; private set; }

        public string FileName { get; private set; }

        public string[] FileNames { get; set; }

        private OpenFileResult()
        {
        }

        public OpenFileResult(string fileName)
        {
            PerformOpen = true;
            FileName = fileName;
        }

        public OpenFileResult(string[] fileNames)
        {
            PerformOpen = true;
            FileNames = fileNames;
        }

        public static OpenFileResult DoNothing
        {
            get
            {
                return new OpenFileResult();
            }
        }
    }
}
