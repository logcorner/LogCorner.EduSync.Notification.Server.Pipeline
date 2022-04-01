using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Common.Authentication
{
    public class IdentityProvider : IIdentityProvider
    {
        private readonly IConfiguration _configuration;

        public IdentityProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> AcquireTokenForConfidentialClient()
        {
            bool.TryParse(_configuration["isAuthenticationEnabled"], out var isAuthenticationEnabled);
            if (!isAuthenticationEnabled)
            {
                return string.Empty;
            }
         
            var domain = _configuration["AzureAdConfidentialClient:TenantName"];
            string defaultScope = string.Empty;
            if (!string.IsNullOrWhiteSpace(domain))
            {
                defaultScope = $"https://{domain}.onmicrosoft.com/signalr/hub/.default";
            }
            var scopes = new[] { defaultScope };

            string tenantId = _configuration["AzureAdConfidentialClient:TenantId"];
            string authority = $"https://login.microsoftonline.com/{tenantId}";

            string clientSecret = _configuration["AzureAdConfidentialClient:ClientSecret"];

            string clientId = _configuration["AzureAdConfidentialClient:ClientId"];

            var app =
                       ConfidentialClientApplicationBuilder.Create(clientId)
                           .WithClientSecret(clientSecret)
                           .WithAuthority(new Uri(authority))
                           .Build();

            AuthenticationResult result;
            try
            {
                result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token acquired \n");
                Console.ResetColor();
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Scope provided is not supported");
                Console.ResetColor();
                throw;
            }

            return result?.AccessToken;
        }
    }
}