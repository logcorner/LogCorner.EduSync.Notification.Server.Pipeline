using LogCorner.EduSync.Notification.Common.Hub;
using LogCorner.EduSync.Notification.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Common.IntegrationTests
{
    public class HubConnectionInstanceMock : IHubInstance
    {
        public HubConnection Connection { get; private set; }

        private async Task InitConfidentialClientAsync()
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSignalR();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<LogCornerHub<object>>("/logcornerhub");
                    });
                });

            var server = new TestServer(webHostBuilder);
            Connection = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/logcornerhub",
                    o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .Build();

            await Task.CompletedTask;
        }

        public async Task StartAsync()
        {
            await InitConfidentialClientAsync();
            await Connection.StartAsync();
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}