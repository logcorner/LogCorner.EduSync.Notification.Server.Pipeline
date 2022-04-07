using LogCorner.EduSync.Notification.Common.Hub;
using LogCorner.EduSync.Speech.Telemetry;
using Microsoft.AspNetCore.SignalR;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Server.Hubs
{
    public class LogCornerHub<T> : Hub<IHubNotifier<T>>, IHubInvoker<T> where T : class
    {
        private static readonly ActivitySource Activity = new("notification-server");
        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();
        private Client Client => GetClientName();

        private ITraceService _traceService;

        public LogCornerHub(ITraceService traceService)
        {
            _traceService = traceService;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"OnConnectedAsync :: clientId : {Context.ConnectionId}, clientName : {Client.ClientName}, User : {Client.ConnectedUser} - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"OnDisconnectedAsync :: clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Subscribe(string topic)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, topic);
            await Clients.Groups(topic).OnSubscribe(Context.ConnectionId, topic);
            Console.WriteLine($"Subscribe :: topic : {topic} , clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
        }

        public async Task Publish(T payload)
        {
            await Clients.All.OnPublish(payload);
            Console.WriteLine($"Publish :: payload : {payload} , clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
        }

        public async Task PublishToTopic(string topic, IDictionary<string, string> headers, T payload)
        {
            var parentContext = Propagator.Extract(default, headers, _traceService.ExtractTraceContextFromBasicProperties);
            Baggage.Current = parentContext.Baggage;

            using (var activity =
                   Activity.StartActivity("Process Message", ActivityKind.Server, parentContext.ActivityContext))
            {
                var tags = new Dictionary<string, object>
                {
                    {"topic", topic },
                    {"headers", string.Join("|", headers.Select(h=> $"({h.Key},{h.Value})"))} ,
                    {"payload", payload.ToString()}
                };
                _traceService.SetActivityTags(activity, tags);
                await Clients.All.OnPublish(topic, headers, payload);
                Console.WriteLine(
                    $"PublishToTopic :: topic : {topic} , payload : {payload}, clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
            }
        }

   

        public async Task UnSubscribe(string topic)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, topic);
            await Clients.Groups(topic).OnUnSubscribe(Context.ConnectionId, topic);
            Console.WriteLine($"UnSubscribe :: topic : {topic} , clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
        }

        private Client GetClientName()
        {
            var httpContext = Context.GetHttpContext();

            var clientName = httpContext.Request.Query["clientName"];
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new Exception($"clientName is required ** OnConnectedAsync :: clientId : {Context.ConnectionId}");
            }
            return new Client(httpContext, clientName);
        }
    }
}