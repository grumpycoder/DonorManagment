using CsvHelper.Configuration;
using System;
using Web.Models;
using Web.Utilities;

namespace Web.Infrastructure.Mapping
{
    public sealed class CsvMap : CsvClassMap<CsvTaxRecordViewModel>
    {
        public CsvMap()
        {
            Map(m => m.LookupId).Name("LookupID", "Lookup ID", "LookupId");
            Map(m => m.FinderNumber).Name("FinderNumber", "findernumber", "Finder Number", "finder number");
            Map(m => m.Name).Name("Name");
            Map(m => m.EmailAddress).Name("EmailAddress", "Email address");
            Map(m => m.Addressline1).Name("Addressline1", "Address Line 1", "Address line 1");
            Map(m => m.Addressline2).Name("Addressline2", "Address Line 2", "Address line 2");
            Map(m => m.Addressline3).Name("Addressline3", "Address Line 3", "Address line 3");
            Map(m => m.City).Name("City");
            Map(m => m.State).Name("State");
            Map(m => m.Zipcode).Name("Zip", "zip", "ZIP", "Zipcode", "zipcode", "ZipCode");
            Map(m => m.TaxYear).ConvertUsing(row => row.GetField("Date").ToDateTime().Year);
            Map(m => m.DonationDate).Name("Date");
            Map(m => m.Amount)
                .Name("Amount")
                .ConvertUsing(row => Convert.ToDecimal(row.GetField("Amount").Replace("$", "")));

        }
    }
}