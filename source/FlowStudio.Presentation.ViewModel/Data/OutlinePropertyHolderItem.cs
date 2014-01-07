// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel.Data
{
    public class OutlinePropertyHolderItem : OutlineItem
    {
        private string propertyName;

        public string PropertyName
        {
            get { return propertyName; }
            set { Set(() => PropertyName, ref propertyName, value); }
        }

        public OutlinePropertyHolderItem(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
