
namespace TouchpointMedical.Infrastructure
{
    public interface IWebhookMessageList : IWebhookMessage
    {
        IList<string> Items { get; }
    }
}
