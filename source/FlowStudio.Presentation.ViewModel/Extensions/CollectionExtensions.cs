// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionExtensions
    {
        public static void RemoveItems<T>(this ICollection<T> collection,
                                          IList<T> itemsToRemove)
        {
            if (collection == null)
            {
                return;
            }

            for (var i = collection.Count - 1; i >= 0; i--)
            {
                var item = collection.ElementAt(i);

                if (itemsToRemove.Contains(item))
                {
                    collection.Remove(item);
                }
            }
        }
    }
}
