// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Data
{
    public class ReferencePathItem
    {
        public bool IsSelected { get; set; }

        public string DirectoryPath { get; private set; }

        public ReferencePathItem(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public override string ToString()
        {
            return DirectoryPath;
        }
    }
}
