// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel
{
    using System;
    using Common;

    public class StatusViewModel : ViewModel
    {
        private static StatusViewModel instance;

        public static StatusViewModel Instance
        {
            get { return instance ?? (instance = new StatusViewModel()); }
        }

        private StatusViewModel()
        {
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { Set(() => Text, ref text, value); }
        }

        private string progressText;

        public string ProgressText
        {
            get { return progressText; }
            set { Set(() => ProgressText, ref progressText, value); }
        }

        private int? progressValue;

        public int? ProgressValue
        {
            get { return progressValue; }
            set { Set(() => ProgressValue, ref progressValue, value); }
        }

        public static void SetText(string statusText)
        {
            Instance.Text = statusText;
        }

        public static void SetProgressText(string progressText)
        {
            Instance.ProgressText = progressText;
        }

        public static void SetProgressValue(int currentValue,
                                            int maximumValue)
        {
            Instance.ProgressValue = (int)Math.Floor((decimal)currentValue*100/maximumValue);
        }

        public static void HideProgress()
        {
            Instance.ProgressText = null;
            Instance.ProgressValue = null;
        }
    }
}
