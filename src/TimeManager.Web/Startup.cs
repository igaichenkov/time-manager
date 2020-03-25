using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using TimeManager.Web.Data;
using TimeManager.Web.Models.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TimeManager.Web.Services;
using TimeManager.Web.ActionFilters;
using TimeManager.Web.DbErrorHandlers;
using TimeManager.Web.Data.Identity;

namespace TimeManager.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(
                    AuthPolicies.WorkEntriesAccess,
                    policy => policy.RequireRole(RoleNames.Admin, RoleNames.User));

                opt.AddPolicy(
                    AuthPolicies.ManageUsers,
                    policy => policy.RequireRole(RoleNames.Admin, RoleNames.Manager)
                    );
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/signin";
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = (context) =>
                {
                    if (context.HttpContext.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = 401;
                    }

                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = (context) =>
                {
                    if (context.HttpContext.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = 403;
                    }

                    return Task.CompletedTask;
                };
            });

            RegisterCustomServices(services);
        }

        private static void RegisterCustomServices(IServiceCollection services)
        {
            services.AddScoped<IWorkEntriesService, WorkEntriesService>();
            services.AddScoped<ArgumentExceptionHandlerAttribute>();
            services.AddSingleton<IDbErrorHandler, SqliteErrorHandler>();
            services.AddScoped<IUserContextAccessor, UserContextAccessor>();
            services.AddScoped<AuthInitializer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            ApplicationDbContext context,
            AuthInitializer authInitializer,
            ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                logger.LogInformation("Applying migrations...");
                context.Database.Migrate();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            authInitializer.SeedRolesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
