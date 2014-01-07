// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Build.Execution
{
    using System;
    using System.ComponentModel;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class WorkflowRunner : IWorkflowRunner
    {
        public bool IsRunning { get; private set; }

        protected void OnExecutingStateChanged(WorkflowExecutingStateEventArgs e)
        {
            IsRunning = e.IsExecuting;

            if (ExecutingStateChanged != null)
            {
                ExecutingStateChanged(this, e);
            }
        }

        public abstract void Start();

        public abstract void Stop();

        public event EventHandler<WorkflowExecutingStateEventArgs> ExecutingStateChanged;

        protected string Xaml { get; private set; }

        protected TextWriter OutputWriter { get; private set; }

        protected WorkflowRunner(string xaml, TextWriter outputWriter)
        {
            Xaml = xaml;
            OutputWriter = outputWriter;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
