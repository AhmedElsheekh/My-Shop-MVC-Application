
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.DAL.Configurations;
using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shop.DAL.Context
{
    public class ShopDbContext : IdentityDbContext<ApplicationUser>
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(t => t.Name).HasMaxLength(128).IsRequired();
                entity.Property(t => t.LoginProvider).HasMaxLength(128).IsRequired();
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(l => l.ProviderKey).HasMaxLength(128).IsRequired();
                entity.Property(l => l.LoginProvider).HasMaxLength(128).IsRequired();
            });

            builder.ApplyConfiguration(new ProductConfig());
            builder.ApplyConfiguration(new OrderHeaderConfig());
            builder.ApplyConfiguration(new OrderDetailConfig());
            base.OnModelCreating(builder);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

    }
}
