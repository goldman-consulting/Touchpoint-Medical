namespace TouchpointMedical.Infrastructure
{
    public interface IMessageQueue
    {
        Task EnqueueMessageAsync(IWebhookMessage message);
        Task<IWebhookMessage?> ReceiveNextMessageAsync();
        Task RequeueAsync(IWebhookMessage message);
    }
}
