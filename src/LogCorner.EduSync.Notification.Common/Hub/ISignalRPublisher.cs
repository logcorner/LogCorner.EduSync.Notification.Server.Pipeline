using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Common.Hub
{
    public interface ISignalRPublisher
    {
        Task SubscribeAsync(string topic);

        Task PublishAsync<T>(string topic, T payload);
    }
}