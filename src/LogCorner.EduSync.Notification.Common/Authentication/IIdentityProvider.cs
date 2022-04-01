using System.Threading.Tasks;

namespace LogCorner.EduSync.Notification.Common.Authentication
{
    public interface IIdentityProvider
    {
        Task<string> AcquireTokenForConfidentialClient();
    }
}