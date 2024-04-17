using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using SmartSheetApi = Smartsheet.Api;
using SmartSheetLoader.Services;
using Syncfusion.Blazor;
using SmartsheetClientSdk = Smartsheet.Api.SmartsheetClient;

namespace SmartSheetLoader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("SyncFusion:keyValue"));
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSyncfusionBlazor();
            var myApiSettings = builder.Configuration.GetValue<string>("SmartSheetToken");
            builder.Services.AddHttpClient("smartsheet", client =>
            {
                
                var baseAddress = builder.Configuration.GetValue<string>("BaseAddress");
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", myApiSettings);
                //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {myApiSettings}");
            });
            builder.Services.AddSingleton<SmartsheetClientSdk>(provider =>
            {

                // Create and return an instance of SmartsheetClient
                return new SmartSheetApi.SmartsheetBuilder().SetAccessToken(myApiSettings).Build();
            });
            builder.Services.AddScoped<ISmartsheetClient, SmartsheetClient>();
            builder.Services.AddScoped<ICsvProcessor, CsvProcessor>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}