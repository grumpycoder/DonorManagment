using System.Collections.Generic;
using System.Linq;

namespace Web.Models
{
    public class SearchViewModel
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string OrderBy { get; set; }
        public string OrderDirection { get; set; }

        public string Name { get; set; }
        public string FinderNumber { get; set; }
        public string LookupId { get; set; }
        public bool? IsUpdated { get; set; }

        public int TotalCount { get; set; }
        public int Count => Items?.Count() ?? 0;
        public int FilteredCount { get; set; }
        public int TotalPages { get; set; }

        public IList<ConstituentViewModel> Items { get; set; }
    }
}