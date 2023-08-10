
namespace Application.Tests.MyTestedApp;

using Application.Data.Models;
using Microsoft.EntityFrameworkCore;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Application.Data;
using Application.Web.Infrastructure.Extensions;
using Application.Services.Mapping;
using Application.Web.ViewModels;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class TestStartUp
{
    public void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //var connectionString = builder.Configuration["ConnectionString"];

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 5;
        })
            .AddRoles<IdentityRole>() // needed to using roles
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews()
            .AddMvcOptions(options =>
            {
                options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            });

        // Dependencies registrator made by Christian Ivanov, Softuni Bulgaria

        builder.Services.AddApplicationServices(typeof(IPortfolioService));

        builder.Services.AddMemoryCache();

        builder.Services.ConfigureApplicationCookie(cfg =>
        {
            cfg.LoginPath = "/User/Login";
        });

        var app = builder.Build();

        AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

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

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // adding areas
        app.UseEndpoints(endpoints =>
        {
            //endpoints.MapAreaControllerRoute(
            //  name: "areas",
            //  areaName:"Admin",
            //  pattern: "Admin/{controller=Role}/{action=Index}"
            //);
            endpoints.MapControllerRoute(
            name: "Areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );
            endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapDefaultControllerRoute();
            endpoints.MapRazorPages();
        });


        app.Run();
    }
}
