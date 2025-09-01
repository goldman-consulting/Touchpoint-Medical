using Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace TouchpointMedical.Infrastructure.Azure
{
    public class AzureMessageQueue(ServiceBusClient client, IOptions<WebhookListenerOptions> queueOptions) : IMessageQueue
    {
        private readonly ServiceBusSender _sender = client.CreateSender(queueOptions.Value.QueueName);
        private readonly ServiceBusReceiver _receiver = client.CreateReceiver(queueOptions.Value.QueueName);

        public async Task EnqueueMessageAsync(IWebhookMessage message)
        {
            var body = JsonSerializer.Serialize(message);
            var msg = new ServiceBusMessage(body)
            {
                MessageId = Guid.NewGuid().ToString(),
                Subject = message.MessageId
            };

            await _sender.SendMessageAsync(msg);
        }

        public async Task<IWebhookMessage?> ReceiveNextMessageAsync()
        {
            var msg = await _receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
            if (msg == null)
            {
                return null;
            }

            var body = msg.Body.ToString();
            var webhookMessage = JsonSerializer.Deserialize<IWebhookMessageList>(body);
            webhookMessage!.MessageData = msg;
            return webhookMessage;
        }

        public async Task RequeueAsync(IWebhookMessage message)
        {
            await EnqueueMessageAsync(message);
        }
    }
}
