// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class OutlineItemObservableCollection : ObservableCollection<OutlineItem>
    {
        public OutlineItemObservableCollection(IEnumerable<OutlineItem> collection)
            : base(collection)
        {
        }

        public OutlineItemObservableCollection(List<OutlineItem> list)
            : base(list)
        {
        }

        public OutlineItemObservableCollection()
        {
        }
    }
}
