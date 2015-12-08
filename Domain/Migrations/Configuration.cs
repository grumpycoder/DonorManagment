using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;

namespace Domain.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AppContext context)
        {
            //  This method will be called after migrating to the latest version.

            context.Constituents.AddOrUpdate(c => c.LookupId, new Constituent()
            {
                Name = "Heather Bossin",
                LookupId = "10782387129",
                TaxItems = new List<TaxItem>()
                {
                    new TaxItem() {TaxYear = 2014, Amount = 25, DonationDate = DateTime.Parse("2/3/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 100, DonationDate = DateTime.Parse("12/12/2014")}
                }
            });

            context.Constituents.AddOrUpdate(c => c.LookupId, new Constituent()
            {
                Name = "Mervin Slobodin",
                LookupId = "10972679863",
                TaxItems = new List<TaxItem>()
                {
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("1/24/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("2/23/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("3/22/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("4/22/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("5/22/2014")},
                }
            });

            context.Constituents.AddOrUpdate(c => c.LookupId, new Constituent()
            {
                Name = "Eugene Rogers",
                LookupId = "10972692270",
                TaxItems = new List<TaxItem>()
                {
                    new TaxItem() {TaxYear = 2014, Amount = 14, DonationDate = DateTime.Parse("1/24/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 14, DonationDate = DateTime.Parse("2/24/2014")}
                }
            });

            context.Templates.AddOrUpdate(t => t.Name, new Template()
            {
                Name = "DonorTax",
                HeaderText = "**Tax Receipt**",
                BodyText = "To view and print your tax receipt for donations made to SPLC, please enter the Supporter ID number that was provided to you. Thank you for your generous support of the Southern Poverty Law Center.",
                FAQText = "*This will serve as your receipt for tax purposes. The Southern Poverty Law Center is a 501(c)(3) organization. Gifts to the Center are fully tax-deductible. No goods or services are ever sent in exchange for gifts.*"
            });

        }
    }
}
