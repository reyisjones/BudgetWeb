using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SpaServices.Extensions;

namespace BudgetWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Enable Electron for desktop app functionality
        builder.WebHost.UseElectron(args);
        
        // Add CORS policy for development
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowViteDev", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });
        
        // Add services to the container
        builder.Services.AddControllersWithViews();
        
        // Add SPA static files
        builder.Services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "ClientApp/build";
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSpaStaticFiles();
        
        app.UseCors("AllowViteDev");
        
        app.UseRouting();

        app.MapControllers();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";

            if (app.Environment.IsDevelopment())
            {
                // Don't use proxy in development - let Vite handle it
                // spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
            }
        });

        // Open Electron window if running as desktop app
        Task.Run(async () =>
        {
            var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = 1280,
                Height = 800,
                Title = "Budget App",
                AutoHideMenuBar = false,
                WebPreferences = new WebPreferences
                {
                    NodeIntegration = false,
                    ContextIsolation = true
                }
            });
        });

        app.Run();
    }
}

