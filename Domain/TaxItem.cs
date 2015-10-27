using System;

namespace Domain
{
    public class TaxItem
    {
        public int Id { get; set; }
        public string ConstituentId { get; set; }
        public int TaxYear { get; set; }
        public DateTime? DonationDate { get; set; }
        public decimal Amount { get; set; }

        public virtual Constituent Constituent { get; set; }

    }
}