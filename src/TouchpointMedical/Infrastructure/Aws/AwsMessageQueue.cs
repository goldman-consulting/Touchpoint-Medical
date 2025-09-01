using Amazon.SQS;
using Amazon.SQS.Model;

using Microsoft.Extensions.Options;

using System.Text.Json;

namespace TouchpointMedical.Infrastructure.Aws
{
    public class AwsMessageQueue(IAmazonSQS sqs, IOptions<WebhookListenerOptions> queueOptions) : IMessageQueue
    {
        private readonly IAmazonSQS _sqs = sqs;
        private readonly string _queueUrl = queueOptions.Value.QueueName!;

        public async Task EnqueueMessageAsync(IWebhookMessage message)
        {
            var body = JsonSerializer.Serialize(message);
            await _sqs.SendMessageAsync(new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = body,
                MessageGroupId = message.MessageId // for FIFO support
            });
        }

        public async Task<IWebhookMessage?> ReceiveNextMessageAsync()
        {
            var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 5
            });

            if (response.Messages.Count == 0) return null;

            Message sqsMessage = response.Messages[0];
            var message = JsonSerializer.Deserialize<IWebhookMessageList>(sqsMessage.Body);
            message!.MessageData = sqsMessage.ReceiptHandle;
            return message;
        }

        public async Task RequeueAsync(IWebhookMessage message)
        {
            await EnqueueMessageAsync(message);
        }
    }
}
