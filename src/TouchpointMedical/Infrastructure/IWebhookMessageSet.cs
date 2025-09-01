
namespace TouchpointMedical.Infrastructure
{
    public interface IWebhookMessageSet : IWebhookMessage
    {
        IDictionary<string, object?> Items { get; }
    }
}
