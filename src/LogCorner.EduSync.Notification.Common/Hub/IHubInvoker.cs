using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Common.Hub
{
    public interface IHubInvoker<in T> where T : class
    {
        Task Publish(T payload);

        Task PublishToTopic(string topic, T payload);

        Task Subscribe(string topic);

        Task UnSubscribe(string topic);
    }
}