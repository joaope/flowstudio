// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.ViewModel.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SelectActivitiesTypesResult
    {
        public bool PerformAdd { get; private set; }

        public Type[] SelectedActivitiesTypes { get; private set; }

        public SelectActivitiesTypesResult(IEnumerable<Type> selectedActivitiesTypes)
        {
            SelectedActivitiesTypes = selectedActivitiesTypes.ToArray();

            PerformAdd = SelectedActivitiesTypes.Length != 0;
        }

        public SelectActivitiesTypesResult()
        {
        }

        public static SelectActivitiesTypesResult DoNothing
        {
            get
            {
                return new SelectActivitiesTypesResult();
            }
        }
    }
}
