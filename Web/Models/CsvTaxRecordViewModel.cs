using CsvHelper.Configuration;
using Web.Infrastructure.Mapping;

namespace Web.Models
{
    public class CsvTaxRecordViewModel : CsvClassMap<CsvMap>
    {
        public string LookupId { get; set; }
        public int ConstituentId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; }
        public string Addressline3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string DonationDate { get; set; }
        public decimal Amount { get; set; }
        public int TaxYear { get; set; }
    }
}