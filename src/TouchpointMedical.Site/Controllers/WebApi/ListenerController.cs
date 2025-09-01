using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using TouchpointMedical.Integration.PointClickCare.Configuration;
using TouchpointMedical.Integration.PointClickCare.Infrastructure;

namespace TouchpointMedical.Site.Controllers.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListenerController(
        ILogger<HomeController> logger, 
        PointClickCareRedisMessageQueue messageQueue, 
        IOptions<PointClickCareOptions> pointClickCareOptions,
        IWebHostEnvironment env) : ControllerBase
    {
        private readonly PointClickCareRedisMessageQueue _messageQueue = messageQueue;
        private readonly ILogger<HomeController> _logger = logger;
        private readonly PointClickCareOptions _pointClickCareOptions = pointClickCareOptions.Value;
        private readonly IWebHostEnvironment _env = env;

        [HttpPost]
        public async Task<JsonResult> Receive([FromBody] PointClickCareWebhookMessage message)
        {
            var status = "Ok";

            try
            {
                var useDevLogging = _env.IsDevelopment()
                        || _env.IsEnvironment("UAT-Gateway");

                if (!ModelState.IsValid)
                {
                    if (useDevLogging && Request.HttpContext.Items.ContainsKey("RawBody"))
                    {
                        var rawBody = Request.HttpContext.Items["RawBody"];

                        // log it (masking sensitive fields!)
                        _logger.LogWarning("Invalid webhook payload: {Body}", rawBody);

                        return new JsonResult(new
                        {
                            status = "Invalid webhook payload",
                            rawBody,
                            Request.Headers
                        });
                    }
                }

                var validationResult = message.Validate(_pointClickCareOptions, Request.Headers);

                switch (validationResult)
                {
                    case PointClickCareWebhookMessage.ValidationResultType.Unauthorized:

                        _logger.LogWarning(
                            "{Message} {Webhook}", "Webhook message not valid: Ensure credentials and types are valid.",
                            message);

                        if (useDevLogging)
                        {
                            status = "Invalid credentials.";
                        }

                        break;
                    case PointClickCareWebhookMessage.ValidationResultType.Unqualified:
                        _logger.LogTrace(
                            "{Message} {@Webhook}", "Webhook Notification Received (Not qualified)",
                            message);

                        if (useDevLogging)
                        {
                            status = "Invalid event type.";
                        }

                        break;
                    case PointClickCareWebhookMessage.ValidationResultType.Qualified:
                    default:
                        _logger.LogDebug(
                            "{Message} {@Webhook}", "Webhook Notification Received (Qualified)",
                            message);

                        await _messageQueue.EnqueueMessageAsync(message);

                        status = "Queued";

                        break;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("{Message} {Error}", 
                    "Unknown exception receiving webhook notification",
                    ex);
            }

            return new JsonResult(new { status });
        }
    }
}
