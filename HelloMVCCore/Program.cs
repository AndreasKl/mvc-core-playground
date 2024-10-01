using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HelloMVCCore.Data;
using HelloMVCCore.Infrastructure;
using HelloMVCCore.Infrastructure.MVC;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace HelloMVCCore;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var services = builder.Services;
        
        services.AddTransient<IClaimsTransformation, TestClaimsTransformation>();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddSystemWebAdapters();
       

        // TODO: Validate what kind of dependencies are registered by which part and if AddIdentityCookies is needed.
        services.AddAuthentication(o =>
        {
            o.DefaultScheme = IdentityConstants.ApplicationScheme;
            o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        }).AddIdentityCookies(o => { });
        
        services.AddIdentityCore<ApplicationUser>(o =>
            {
                o.Stores.MaxLengthForKeys = 128;
                o.SignIn.RequireConfirmedAccount = true;
                o.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            // Add obviously the default UI
            //.AddDefaultUI()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddTransient<IEmailSender, NoOpEmailSender>();
        
        // Not needed but, a possible extension point...
        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.FromSeconds(10);
            options.OnRefreshingPrincipal = context =>
            {
                // Same as our refresh stuff in nom
                var user = context.CurrentPrincipal;
                return Task.CompletedTask;
            };
        });

        // Setup legacy ws federation as in the old software
        services.AddAuthentication().AddWsFederation(options =>
        {
            options.Wtrealm = builder.Configuration["wsfed:realm"];
            options.MetadataAddress = builder.Configuration["wsfed:metadata"];
            options.AllowUnsolicitedLogins = false;
        });

        // See: Microsoft.AspNetCore.Identity.UI.IdentityDefaultUIConfigureOptions[TUser].Configure
        // for reference.
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.AccessDeniedPath = "/identity/account/AccessDenied";
        });
        
        // Just a noop sample...
        services.AddAuthorization(options =>
        {
            options.AddPolicy("SomePolicy", policyBuilder => policyBuilder.Requirements.Add(new SomeAuthorizationRequirement()));
        });

        // Localization..
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "en-US", "de-DE" };
            options.SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
        });
        
        // MVC filters with DI
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add<ProjectProvidingFilter>();
        });
        services.AddRazorPages(options =>
        {
            // Check if we need sth from:
            // Microsoft.AspNetCore.Identity.UI.IdentityDefaultUIConfigureOptions[TUser].PostConfigure
            options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            // Role management...
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>()!;
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