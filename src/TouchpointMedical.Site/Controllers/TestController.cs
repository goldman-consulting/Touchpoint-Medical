using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Wamley.Site.Controllers
{
    public class TestController(
        ILogger<TestController> logger, 
        IAmazonS3 s3Client, 
        IAmazonSQS sqsClient,
        IConfiguration configuration) : Controller
    {
        private readonly ILogger<TestController> _logger = logger;
        private readonly IAmazonS3 _s3Client = s3Client;
        private readonly IAmazonSQS _sqsClient = sqsClient;
        private readonly string _bucketName = configuration["AWSOptions:BucketName"] ?? "";

        public IActionResult Index()
        {
            _logger.LogTrace("");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or missing.");
            }

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = file.FileName,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };

            try
            {
                var response = await _s3Client.PutObjectAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok($"Successfully uploaded {file.FileName} to S3.");
                }
                else
                {
                    return StatusCode((int)response.HttpStatusCode, "Failed to upload file to S3.");
                }
            }
            catch (AmazonS3Exception e)
            {
                return StatusCode((int)e.StatusCode, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string queueUrl, string messageBody)
        {
            if (string.IsNullOrEmpty(queueUrl) || string.IsNullOrEmpty(messageBody))
                return BadRequest("Queue URL or message body is missing.");

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            try
            {
                var response = await _sqsClient.SendMessageAsync(sendMessageRequest);
                return Ok($"Message sent with ID: {response.MessageId}");
            }
            catch (AmazonSQSException e)
            {
                return StatusCode((int)e.StatusCode, e.Message);
            }
        }
    }
}
