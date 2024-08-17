
using Microsoft.AspNetCore.Identity;
using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.DAL.Context
{
    public static class ShopDbContextSeed
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("Editor"));
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            if(!userManager.Users.Any())
            {
                await userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "admin@myshop.com",
                    Email = "admin@myshop.com",
                    Name = "Ahmed Elsheekh",
                    City = "Sohag",
                    Address = "Ahmed Oraby Street",
                    PhoneNumber = "01121452354"
                }, "P@ssw0rd");

                ApplicationUser user = await userManager.FindByEmailAsync("admin@myshop.com");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
