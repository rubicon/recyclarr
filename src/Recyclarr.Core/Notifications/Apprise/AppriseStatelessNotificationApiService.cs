using Flurl.Http;
using Recyclarr.Notifications.Apprise.Dto;
using Recyclarr.Settings;

namespace Recyclarr.Notifications.Apprise;

public class AppriseStatelessNotificationApiService(IAppriseRequestBuilder api) : IAppriseNotificationApiService
{
    public async Task Notify(
        AppriseNotificationSettings settings,
        Func<AppriseNotification, AppriseNotification> notificationBuilder)
    {
        if (settings.Urls is null)
        {
            throw new ArgumentException("Stateless apprise notifications require the 'urls' array");
        }

        var notification = notificationBuilder(new AppriseStatelessNotification
        {
            Urls = settings.Urls
        });

        await api.Request("notify").PostJsonAsync(notification);
    }
}