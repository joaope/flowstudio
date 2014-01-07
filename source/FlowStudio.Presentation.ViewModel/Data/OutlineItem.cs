// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel.Data
{
    using GalaSoft.MvvmLight;

    public abstract class OutlineItem : ObservableObject
    {
        private bool isExpanded;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { Set(() => IsExpanded, ref isExpanded, value); }
        }

        private OutlineItemObservableCollection innerItems;

        public OutlineItemObservableCollection InnerItems
        {
            get { return innerItems; }
            set { Set(() => InnerItems, ref innerItems, value); }
        }

        protected OutlineItem()
        {
            IsExpanded = true;
            InnerItems = new OutlineItemObservableCollection();
        }
    }
}
