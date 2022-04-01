using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Server
{
    public static class ServicesConfiguration
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            bool.TryParse(configuration["isAuthenticationEnabled"], out var isAuthenticationEnabled);
            if (!isAuthenticationEnabled)
            {
                return;
            }

            services
                .AddAuthentication()
                .AddJwtBearer("AAD", options =>
                {
                    options.Authority = $"{configuration["AzureAd:Instance"]}/{configuration["AzureAd:TenantId"]}/v2.0";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = $"{configuration["AzureAd:Instance"]}/{configuration["AzureAd:TenantId"]}/v2.0",
                        ValidateAudience = true,
                        ValidAudience = configuration["AzureAd:ClientId"],
                        ValidateLifetime = true,
                        NameClaimType = "name"
                    };
                })

                .AddJwtBearer("B2C", options =>
                {
                    options.Authority = $"{configuration["AzureAdB2C:Instance"]}/tfp/{configuration["AzureAdB2C:TenantId"]}/{configuration["AzureAdB2C:SignUpSignInPolicyId"]}/v2.0/";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = $"{configuration["AzureAdB2C:Instance"]}/{configuration["AzureAdB2C:TenantId"]}/v2.0/",
                        ValidateAudience = true,
                        ValidAudience = configuration["AzureAdB2C:ClientId"],
                        ValidateLifetime = true,
                        NameClaimType = "name"
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/logcornerhub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            services
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("AAD", "B2C")
                        .Build();

                    options.AddPolicy("AADAdmins", new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("AAD")

                        .Build());
                });
        }
    }
}