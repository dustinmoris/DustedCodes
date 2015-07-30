using System.Collections.Generic;

namespace DustedCodes.Core.Collections
{
    public sealed class PagedCollection<T>
    {
        public ICollection<T> Items { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}