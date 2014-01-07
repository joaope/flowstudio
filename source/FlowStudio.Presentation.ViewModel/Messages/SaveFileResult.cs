// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Messages
{
    public class SaveFileResult
    {
        public string FileName { get; private set; }

        public bool PerformSave { get; private set; }

        private SaveFileResult()
        {
        }

        public SaveFileResult(string fileName)
        {
            PerformSave = true;
            FileName = fileName;
        }

        public static SaveFileResult DoNothing
        {
            get
            {
                return new SaveFileResult();
            }
        }
    }
}
