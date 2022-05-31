using LogCorner.EduSync.Notification.Common.Exceptions;
using LogCorner.EduSync.Notification.Common.Hub;
using LogCorner.EduSync.Speech.Telemetry;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Server.Hubs
{
    public class LogCornerHub<T> : Hub<IHubNotifier<T>>, IHubInvoker<T> where T : class
    {
        private readonly ActivitySource _activity = new("notification-server");
        private readonly TextMapPropagator _propagator = new TraceContextPropagator();
        private Client Client => GetClientName();

        private readonly ITraceService _traceService;
        private readonly ILogger<LogCornerHub<T>> _logger;

        public LogCornerHub(ITraceService traceService, ILogger<LogCornerHub<T>> logger)
        {
            _traceService = traceService;
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
#pragma warning disable CA2254 // Template should be a static expression
            _logger.LogInformation($"OnConnectedAsync :: clientId : {Context.ConnectionId}, clientName : {Client.ClientName}, User : {Client.ConnectedUser} - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
#pragma warning disable CA2254 // Template should be a static expression
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {

            _logger.LogInformation($"OnDisconnectedAsync :: clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");

            return base.OnDisconnectedAsync(exception);
        }

        public async Task Subscribe(string topic)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, topic);
            await Clients.Groups(topic).OnSubscribe(Context.ConnectionId, topic);
            _logger.LogInformation($"Subscribe :: topic : {topic} , clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
        }

        public async Task Publish(T payload)
        {
            await Clients.All.OnPublish(payload);
            _logger.LogInformation($"Publish :: payload : {payload} , clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
        }

        public async Task PublishToTopic(string topic, IDictionary<string, string> headers, T payload)
        {
            var parentContext = _propagator.Extract(default, headers, _traceService.ExtractTraceContextFromBasicProperties);
            Baggage.Current = parentContext.Baggage;

            using var activity =
                _activity.StartActivity("Process Message", ActivityKind.Server, parentContext.ActivityContext);
            var tags = new Dictionary<string, object>
            {
                {"topic", topic },
               {"payload", payload.ToString()}
            };
            _traceService.SetActivityTags(activity, tags);
            await Clients.All.OnPublish(topic, headers, payload);
            _logger.LogInformation(
                $"PublishToTopic :: topic : {topic} , payload : {payload}, clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
        }

        public async Task UnSubscribe(string topic)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, topic);
            await Clients.Groups(topic).OnUnSubscribe(Context.ConnectionId, topic);
            _logger.LogInformation($"UnSubscribe :: topic : {topic} , clientId : {Context.ConnectionId}, clientName :{Client.ClientName}, User : {Client.ConnectedUser}  - {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}");
        }

        private Client GetClientName()
        {
            var httpContext = Context.GetHttpContext();

            var clientName = httpContext?.Request.Query["clientName"];
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new HubConnectedUserException($"clientName is required ** OnConnectedAsync :: clientId : {Context.ConnectionId}");
            }
            return new Client(httpContext, clientName);
        }
    }
}