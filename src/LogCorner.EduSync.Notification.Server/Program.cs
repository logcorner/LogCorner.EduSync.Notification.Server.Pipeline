using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LogCorner.EduSync.Notification.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var settings = config.Build();
                    bool.TryParse(settings["isAuthenticationEnabled"], out var isAuthenticationEnabled);
                    if (!context.HostingEnvironment.IsDevelopment() && isAuthenticationEnabled)
                    {
                        // Configure Azure Key Vault Connection
                        var uri = settings["AzureKeyVault:Uri"];
                        var clientId = settings["AzureKeyVault:ClientId"];
                        var clientSecret = settings["AzureKeyVault:ClientSecret"];

                        // Check, if Client ID and Client Secret credentials for a Service Principal
                        // have been provided. If so, use them to connect, otherwise let the connection 
                        // be done automatically in the background
                        if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
                            config.AddAzureKeyVault(uri, clientId, clientSecret);
                        else
                            config
                                .AddAzureKeyVault(uri);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}