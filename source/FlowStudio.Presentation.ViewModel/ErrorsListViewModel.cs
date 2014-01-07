// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel
{
    public class ErrorsListViewModel : ToolViewModel
    {
        public const string ErrorsListContentId = "ErrorsListTool";

        public ErrorsListViewModel()
            : base(ErrorsListContentId, "Errors List")
        {
            ContentId = ErrorsListContentId;
        }
    }
}
