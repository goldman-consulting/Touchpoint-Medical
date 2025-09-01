namespace TouchpointMedical.Infrastructure
{
    public interface IWebhookMessage
    {
        string MessageKey { get; }
        string MessageId { get; }
        DateTimeOffset MessageReceivedAt { get;  }
    }
}
