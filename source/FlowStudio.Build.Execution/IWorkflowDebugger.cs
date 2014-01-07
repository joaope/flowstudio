// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Build.Execution
{
    using System;

    public interface IWorkflowDebugger : IWorkflowRunner
    {
        event EventHandler<DebugStepEventArgs> DebugStepAdded;
    }
}
