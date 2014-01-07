// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel
{
    using FlowStudio.Common;

    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel
        {
            get { return TypeLocator.GetInstance<MainWindowViewModel>(); }
        }

        public OptionsViewModel OptionsViewModel
        {
            get { return TypeLocator.GetInstance<OptionsViewModel>(); }
        }
    }
}