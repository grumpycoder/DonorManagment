using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

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
            modelBuilder.Entity<TaxItem>().Property(p => p.DonationDate).HasColumnType("date");
        }

        public DbSet<Donor> Donors { get; set; }
        public DbSet<Constituent> Constituents { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TaxItem> TaxItems { get; set; }

        public override int SaveChanges()
        {
            var selectedEntityList = ChangeTracker.Entries()
                                    .Where(x => x.Entity is BaseEntity &&
                                    (x.State == EntityState.Modified));

            foreach (var entity in selectedEntityList)
            {
                ((BaseEntity)entity.Entity).UpdatedDate = DateTime.Now;
            }

            return base.SaveChanges();
        }
    }


}