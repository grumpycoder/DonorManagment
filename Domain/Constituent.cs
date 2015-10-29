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
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public ICollection<TaxItem> TaxItems { get; set; }
    }
}