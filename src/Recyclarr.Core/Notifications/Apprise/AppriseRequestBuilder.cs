using System.Text.Json;
using System.Text.Json.Serialization;
using Flurl.Http;
using Flurl.Http.Configuration;
using Recyclarr.Http;
using Recyclarr.Settings;

namespace Recyclarr.Notifications.Apprise;

public sealed class AppriseRequestBuilder(
    IFlurlClientCache clientCache,
    ISettings<NotificationSettings> settings,
    IEnumerable<FlurlSpecificEventHandler> eventHandlers)
    : IAppriseRequestBuilder
{
    private readonly Lazy<Uri> _baseUrl = new(() =>
    {
        var apprise = settings.Value.Apprise;
        if (apprise is null)
        {
            throw new ArgumentException("No apprise notification settings have been defined");
        }

        if (apprise.BaseUrl is null)
        {
            throw new ArgumentException("Apprise `base_url` setting is not present or empty");
        }

        return apprise.BaseUrl;
    });

    public IFlurlRequest Request(params object[] path)
    {
        var client = clientCache.GetOrAdd("apprise", _baseUrl.Value.ToString(), Configure);
        return client.Request(path);
    }

    private void Configure(IFlurlClientBuilder builder)
    {
        foreach (var handler in eventHandlers.Select(x => (x.EventType, x)))
        {
            builder.EventHandlers.Add(handler);
        }

        builder.WithSettings(settings =>
        {
            settings.JsonSerializer = new DefaultJsonSerializer(new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)
                }
            });
        });
    }
}