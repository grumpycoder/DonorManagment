using System.Collections;
using System.Collections.Generic;

namespace Domain
{
    public class Constituent: BaseEntity
    {
        public Constituent()
        {
            TaxItems = new List<TaxItem>();
        }
        public string Name { get; set; }
        public string ConstituentId { get; set; }


        public ICollection<TaxItem> TaxItems { get; set; }
    }
}