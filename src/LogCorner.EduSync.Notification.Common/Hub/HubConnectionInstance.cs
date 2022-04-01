using LogCorner.EduSync.Notification.Common.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Common.Hub
{
    public class HubConnectionInstance : IHubInstance
    {
        private readonly IIdentityProvider _identityProvider;

        private readonly string _url;
        public HubConnection Connection { get; private set; }

        public HubConnectionInstance(string url, IIdentityProvider identityProvider)
        {
            _url = url;
            _identityProvider = identityProvider;
        }

        public async Task StartAsync()
        {
            try
            {
                var accessToken = await InitConfidentialClientAsync();

                Connection = new HubConnectionBuilder()
                    .WithUrl(_url, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(accessToken);
                    })
                    .ConfigureLogging(logging =>
                    {
                        // This will set ALL logging to Debug level
                        logging.SetMinimumLevel(LogLevel.Debug);
                    })

                   .WithAutomaticReconnect(new RandomRetryPolicy())
                    .Build();

                await Connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HubConnectionInstance::HubUrl : {_url}");
                Console.WriteLine($"HubConnectionInstance::Message : {ex.Message}");
                Console.WriteLine($"HubConnectionInstance::InnerException.Message : {ex.InnerException?.Message}");
                Console.WriteLine($"HubConnectionInstance::InnerException?.InnerException?.Message : {ex.InnerException?.InnerException?.Message}");
                Console.WriteLine($"HubConnectionInstance::Exception : {ex}");
            }
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
            var accessToken = await _identityProvider.AcquireTokenForConfidentialClient();
            return accessToken;
        }
    }
}