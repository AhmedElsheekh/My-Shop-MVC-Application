using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.DAL.Context;
using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.DAL.DatabaseInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ShopDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ShopDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeDbAsync()
        {
            //Migrations
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Count() > 0)
                    await _dbContext.Database.MigrateAsync();

            }
            catch (Exception ex)
            {

            }

            //User & Roles Seeding
            await ShopDbContextSeed.SeedAsync(_roleManager, _userManager);
    
        }
    }
}
