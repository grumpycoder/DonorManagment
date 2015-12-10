using System.Collections.Generic;
using System.Linq;

namespace Web.Models
{
    public class PagedCollection<T>
    {
        public int Page { get; set; }

        public int Count => Items?.Count() ?? 0;

        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
