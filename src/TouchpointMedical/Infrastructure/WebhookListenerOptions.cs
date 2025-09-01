namespace TouchpointMedical.Infrastructure
{
    public class WebhookListenerOptions
    {
        public required bool Enabled { get; set; }
        public required string Connection { get; set; }
        public required string KeyPrefix { get; set; }
        public long WindowMs { get; set; } = 30_000;
        public long TimeToLiveHr { get; set; } = 24;
        public long ProcessorDelayMs { get; set; } = 2_000;
    }
}
