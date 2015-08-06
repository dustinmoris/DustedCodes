using System;
using System.Collections.Generic;

namespace DustedCodes.Core.Data.Comparer
{
    public sealed class ByPublishDateDescending : IComparer<Article>
    {
        public int Compare(Article x, Article y)
        {
            if (x == null || y == null)
                throw new InvalidOperationException("Cannot compare a null object.");

            if (x.PublishDateTime < y.PublishDateTime)
                return 1;
            
            if (x.PublishDateTime > y.PublishDateTime)
                return -1;

            return 0;
        }
    }
}