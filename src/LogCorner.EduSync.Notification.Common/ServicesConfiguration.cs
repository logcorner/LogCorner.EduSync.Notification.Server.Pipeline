using LogCorner.EduSync.Notification.Common.Authentication;
using LogCorner.EduSync.Notification.Common.Hub;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace LogCorner.EduSync.Notification.Common
{
    public static class ServicesConfiguration
    {
        public static void AddSignalRServices(this IServiceCollection services, string endpoint)
        {
            services.AddSingleton<ISignalRNotifier, SignalRNotifier>();
            services.AddSingleton<ISignalRPublisher, SignalRPublisher>();
            services.AddSingleton<IIdentityProvider, IdentityProvider>();
            services.AddSingleton<IRetryPolicy, RandomRetryPolicy>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHubInstance, HubConnectionInstance>(ctx => new HubConnectionInstance(endpoint,

                ctx.GetRequiredService<IIdentityProvider>(),
                ctx.GetRequiredService<IRetryPolicy>()

            ));
        }
    }
}