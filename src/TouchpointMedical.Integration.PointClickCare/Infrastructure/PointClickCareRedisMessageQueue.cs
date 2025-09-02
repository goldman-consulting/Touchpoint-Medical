using Microsoft.Extensions.Options;

using StackExchange.Redis;

using TouchpointMedical.Configuration;

namespace TouchpointMedical.Integration.PointClickCare.Infrastructure
{
    public class PointClickCareRedisMessageQueue(
        IOptions<TouchpointOptions> touchpointOptions,
        IConnectionMultiplexer redis)
    {
        private readonly TouchpointOptions _touchpointOptions = touchpointOptions.Value;
        private readonly IConnectionMultiplexer _redis = redis;

        public async Task EnqueueMessageAsync(PointClickCareWebhookMessage message)
        {
            if (message != null)
            {
                var tran = _redis.GetDatabase().CreateTransaction();

                var now = DateTimeOffset.UtcNow;
                var nowMs = now.ToUnixTimeMilliseconds();
                var ttl = TimeSpan.FromHours(_touchpointOptions.TimeToLiveHr);
                var dueAt = now.AddMilliseconds(_touchpointOptions.WindowMs);
                var dueAtMs = dueAt.ToUnixTimeMilliseconds();

                _ = tran.StringSetAsync(
                    $"{_touchpointOptions.KeyPrefix}:last:{message.MessageKey}", nowMs, expiry: ttl);

                // 2) what changed
                foreach (var t in message.Items)
                {
                    var tMessage = $"{t.Key}:{t.Value ?? ""}";

                    _ = tran.SetAddAsync($"{_touchpointOptions.KeyPrefix}:types:{message.MessageKey}", tMessage);
                    _ = tran.KeyExpireAsync($"{_touchpointOptions.KeyPrefix}:types:{message.MessageKey}", ttl);
                }

                if (_touchpointOptions.HttpClientOptions.CallLogging.HasAnyFlags(
                    Logging.WebApiCallLoggingType.WebHookNotifications))
                {
                    _ = tran.SetAddAsync($"{_touchpointOptions.KeyPrefix}:webhooklog:{message.MessageKey}", $"{message.Type}:{message.ResourceIds.AsString()}");
                }


                // 3) (re)schedule this patient: due = now + window
                _ = tran.SortedSetAddAsync($"{_touchpointOptions.KeyPrefix}:due", message.MessageKey, dueAtMs);

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
