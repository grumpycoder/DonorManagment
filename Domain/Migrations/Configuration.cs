using System.Collections.Generic;

namespace Domain.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Domain.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Domain.AppContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Constituents.AddOrUpdate(new Constituent()
            {
                Name = "Heather Bossin", 
                ConstituentId = "10782387129", 
                TaxItems = new List<TaxItem>()
                {
                    new TaxItem() {TaxYear = 2014, Amount = 25, DonationDate = DateTime.Parse("2/3/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 100, DonationDate = DateTime.Parse("12/12/2014")}
                }
            });

            context.Constituents.AddOrUpdate(new Constituent()
            {
                Name = "Mervin Slobodin",
                ConstituentId = "10972679863",
                TaxItems = new List<TaxItem>()
                {
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("1/24/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("2/23/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("3/22/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("4/22/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 20, DonationDate = DateTime.Parse("5/22/2014")},
                }
            });

            context.Constituents.AddOrUpdate(new Constituent()
            {
                Name = "Eugene Rogers",
                ConstituentId = "10972692270",
                TaxItems = new List<TaxItem>()
                {
                    new TaxItem() {TaxYear = 2014, Amount = 14, DonationDate = DateTime.Parse("1/24/2014")},
                    new TaxItem() {TaxYear = 2014, Amount = 14, DonationDate = DateTime.Parse("2/24/2014")}
                }
            });

        }
    }
}
