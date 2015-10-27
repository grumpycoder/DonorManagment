using System;
using System.Data.Entity;
using System.Diagnostics;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain
{
    public class AppContext : IdentityDbContext<ApplicationUser>
    {
        public AppContext() : base("name=DefaultConnection")
        {
            Database.Log = msg => Debug.WriteLine(msg);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Properties<string>().Configure(c => c.HasColumnType("varchar"));
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "Security");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles", "Security");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims", "Security");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins", "Security");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", "Security");

            modelBuilder.Entity<Constituent>().HasMany(t => t.TaxItems);

        }

        public DbSet<Donor> Donors { get; set; }
        public DbSet<Constituent> Constituents { get; set; }
    }
}