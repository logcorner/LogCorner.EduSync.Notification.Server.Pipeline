using LogCorner.EduSync.Notification.Common.Authentication;
using LogCorner.EduSync.Notification.Common.Hub;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogCorner.EduSync.Notification.Common
{
    public static class ServicesConfiguration
    {
        public static void AddSignalRServices(this IServiceCollection services, string endpoint, IConfiguration config)
        {
            services.AddSingleton<ISignalRNotifier, SignalRNotifier>();
            services.AddSingleton<ISignalRPublisher, SignalRPublisher>();
            services.AddSingleton<IIdentityProvider, IdentityProvider>();
            services.AddSingleton<IRetryPolicy, RandomRetryPolicy>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHubInstance, HubConnectionInstance>(ctx =>
            {
                var hubConnectionInstance = new HubConnectionInstance(endpoint,

                    ctx.GetRequiredService<IIdentityProvider>(),
                    ctx.GetRequiredService<ILogger<HubConnectionInstance>>(),
                    ctx.GetRequiredService<IRetryPolicy>()

                    );

                return hubConnectionInstance;
            });
        }
    }
}