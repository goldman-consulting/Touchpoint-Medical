namespace TouchpointMedical.Infrastructure
{
    public interface IMessageProcessor
    {
        Task ProcessAsync(IWebhookMessage message);
    }
}
