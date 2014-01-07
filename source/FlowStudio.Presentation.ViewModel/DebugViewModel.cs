// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel
{
    public class DebugViewModel : ToolViewModel
    {
        public const string DebugContentId = "DebugTool";

        public DebugViewModel() 
            : base(DebugContentId, "Debug")
        {
            ContentId = DebugContentId;
        }
    }
}
