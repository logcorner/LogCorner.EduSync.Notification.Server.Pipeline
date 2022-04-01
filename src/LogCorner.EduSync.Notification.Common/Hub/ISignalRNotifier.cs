using System;
using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Common.Hub
{
    public interface ISignalRNotifier
    {
        event Action<string, object> ReceivedOnPublishToTopic;

        Task StartAsync();

        Task OnPublish();

        Task OnPublish(string topic);

        Task StopAsync();
    }
}