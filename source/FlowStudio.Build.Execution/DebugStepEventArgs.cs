// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Build.Execution
{
    using System;

    public class DebugStepEventArgs : EventArgs
    {
        public DebugStep DebugStep { get; private set; }

        public DebugStepEventArgs(DebugStep debugStep)
        {
            DebugStep = debugStep;
        }
    }
}
