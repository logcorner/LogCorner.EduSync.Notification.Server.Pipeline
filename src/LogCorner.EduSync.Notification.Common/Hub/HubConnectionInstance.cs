using LogCorner.EduSync.Notification.Common.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Common.Hub
{
    public class HubConnectionInstance : IHubInstance
    {
        private readonly IIdentityProvider _identityProvider;

        private readonly string _url;
        public HubConnection Connection { get; private set; }

        private readonly IRetryPolicy _retryPolicy;

        public HubConnectionInstance(string url, IIdentityProvider identityProvider, IRetryPolicy retryPolicy)
        {
            _url = url;
            _identityProvider = identityProvider;

            _retryPolicy = retryPolicy;
        }

        public async Task StartAsync()
        {
            var accessToken = await InitConfidentialClientAsync();

            Connection = new HubConnectionBuilder()
                .WithUrl(_url, options => options.AccessTokenProvider = () => Task.FromResult(accessToken))
                .ConfigureLogging(logging =>
                {
                    // This will set ALL logging to Debug level
                    logging.SetMinimumLevel(LogLevel.Debug);
                })

               .WithAutomaticReconnect(_retryPolicy)
                .Build();

            await Connection.StartAsync();
        }

        public async Task StopAsync()
        {
            if (Connection != null && Connection.State != HubConnectionState.Disconnected)
            {
                await Connection.StopAsync();
            }
        }

        private async Task<string> InitConfidentialClientAsync()
        {
            return await _identityProvider.AcquireTokenForConfidentialClient();
        }
    }
}