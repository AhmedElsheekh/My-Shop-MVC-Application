using Microsoft.EntityFrameworkCore;
using Shop.BLL.UnitOfWork;
using Shop.DAL.Context;
using Shop.PL.Helper;
using Shop.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Shop.DAL.DatabaseInitializer;

namespace Shop.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ShopDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("StripeInfo"));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(20);
            })
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ShopDbContext>();

            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddRazorPages();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();


            var app = builder.Build();

            #region Update Database
            //using var scope = app.Services.CreateScope();
            //var services = scope.ServiceProvider;

            //var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            //try
            //{

            //    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            //    await ShopDbContextSeed.SeedAsync(roleManager);
            //}
            //catch (Exception ex)
            //{
            //    var logger = loggerFactory.CreateLogger<Program>();
            //    logger.LogError(ex, "Error While seeding roles in database");
            //}
            #endregion

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeInfo:SecretKey").Get<string>();

            app.UseSession();

            await SeedDB();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
               name: "admin",
               pattern: "{area=Admin}/{controller=Category}/{action=Index}/{id?}");

            app.Run();

            async Task SeedDB()
            {
                using(var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

                    try
                    {
                        await dbInitializer.InitializeDbAsync();
                    }
                    catch(Exception ex)
                    {
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogError(ex, "Error While Initialize Datbase");
                    }
                }
            }
        }
    }
}