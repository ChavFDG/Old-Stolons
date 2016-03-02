using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace Stolons.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<News> News { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillEntry> BillEntrys { get; set; }
        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Product> Producs { get; set; }
        public DbSet<ProductFamilly> ProductFamillys { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<User> StolonsUsers { get; set; }
        public DbSet<WeekBasket> WeekBaskets { get; set; }
        public DbSet<ApplicationConfig> ApplicationConfig { get; set; }

        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
