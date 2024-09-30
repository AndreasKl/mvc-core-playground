using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HelloMVCCore.Data;
using HelloMVCCore.Infrastructure;
using HelloMVCCore.Infrastructure.MVC;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace HelloMVCCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddSystemWebAdapters();

        // TODO: Validate what kind of dependencies are registered by which part and if AddIdentityCookies is needed.
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultScheme = IdentityConstants.ApplicationScheme;
            o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        }).AddIdentityCookies(o => { });
        
        builder.Services.AddIdentityCore<IdentityUser>(o =>
            {
                o.Stores.MaxLengthForKeys = 128;
                o.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<IdentityRole>()
            .AddDefaultUI()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        builder.Services.AddAuthentication().AddWsFederation(options =>
        {
            options.Wtrealm = builder.Configuration["wsfed:realm"];
            options.MetadataAddress = builder.Configuration["wsfed:metadata"];
            options.AllowUnsolicitedLogins = false;
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("SomePolicy", policyBuilder => policyBuilder.Requirements.Add(new SomeAuthorizationRequirement()));
            
        });

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<ProjectProvidingFilter>();
        });
        
        var app = builder.Build();


        using (var scope = app.Services.CreateScope())
        {
            // Add roles for luulz
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
                await roleManager.CreateAsync(new IdentityRole("Guest"));
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseSystemWebAdapters();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();
        
        app.Run();
    }
}