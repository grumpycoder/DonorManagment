using System;

namespace Domain
{
    public class TaxItem: BaseEntity
    {
        public int ConstituentId { get; set; }
        public int TaxYear { get; set; }
        public DateTime? DonationDate { get; set; }
        public decimal Amount { get; set; }

    }
}