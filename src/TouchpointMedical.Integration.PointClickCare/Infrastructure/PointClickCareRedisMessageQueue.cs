using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using StackExchange.Redis;

using TouchpointMedical.Infrastructure;

namespace TouchpointMedical.Integration.PointClickCare.Infrastructure
{
    public class PointClickCareRedisMessageQueue(
        IOptions<WebhookListenerOptions> webhookListenerOptions,
        IConnectionMultiplexer redis)
    {
        private readonly WebhookListenerOptions _webhookListenerOptions = webhookListenerOptions.Value;
        private readonly IConnectionMultiplexer _redis = redis;

        public async Task EnqueueMessageAsync(IWebhookMessage message)
        {
            if (message != null)
            {
                var tran = _redis.GetDatabase().CreateTransaction();

                var now = DateTimeOffset.UtcNow;
                var nowMs = now.ToUnixTimeMilliseconds();
                var ttl = TimeSpan.FromHours(_webhookListenerOptions.TimeToLiveHr);
                var dueAt = now.AddMilliseconds(_webhookListenerOptions.WindowMs);
                var dueAtMs = dueAt.ToUnixTimeMilliseconds();

                _ = tran.StringSetAsync(
                    $"{_webhookListenerOptions.KeyPrefix}:last:{message.MessageKey}", nowMs, expiry: ttl);

                if (message is IWebhookMessageSet messageSet)
                {
                    // 2) what changed
                    foreach (var t in messageSet.Items)
                    {
                        var tMessage = $"{t.Key}:{t.Value ?? ""}";

                        _ = tran.SetAddAsync($"{_webhookListenerOptions.KeyPrefix}:types:{message.MessageKey}", tMessage);
                        _ = tran.KeyExpireAsync($"{_webhookListenerOptions.KeyPrefix}:types:{message.MessageKey}", ttl);
                    }
                }

                // 3) (re)schedule this patient: due = now + window
                _ = tran.SortedSetAddAsync($"{_webhookListenerOptions.KeyPrefix}:due", message.MessageKey, dueAtMs);

                //Commit transaction
                await tran.ExecuteAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(message));
            }
        }

    }
}
