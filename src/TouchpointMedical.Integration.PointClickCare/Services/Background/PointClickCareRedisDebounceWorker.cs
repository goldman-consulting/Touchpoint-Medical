using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using StackExchange.Redis;

using System.Transactions;

using TouchpointMedical.Configuration;
using TouchpointMedical.Infrastructure;
using TouchpointMedical.Integration.PointClickCare.Configuration;
using TouchpointMedical.Integration.PointClickCare.Extensions;
using TouchpointMedical.Integration.PointClickCare.Integration;
using TouchpointMedical.Integration.PointClickCare.Models;

namespace TouchpointMedical.Integration.PointClickCare.Services.Background
{
    public class PointClickCareRedisDebounceWorker(
        IConnectionMultiplexer redis,
        IOptions<WebhookListenerOptions> webhookListenerOptions,
        IOptions<PointClickCareOptions> pointClickCareOptions,
        ILogger<PointClickCareRedisDebounceWorker> logger,
        IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis = redis;
        private readonly ILogger<PointClickCareRedisDebounceWorker> _logger = logger;
        private readonly WebhookListenerOptions _webhookListenerOptions = webhookListenerOptions.Value;
        private readonly PointClickCareOptions _pointClickCareOptions = pointClickCareOptions.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _redis.GetDatabase();

            while (_webhookListenerOptions.Enabled && !stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("PointClickCareRedisDebounceWorker Heartbeat");
                
                using var scope = scopeFactory.CreateScope();
                var pointClickCareApiService = scope.ServiceProvider.GetRequiredService<PointClickCareApiService>();
                var touchpointApiService = scope.ServiceProvider.GetRequiredService<TouchpointApiService>();

                var nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var ttl = TimeSpan.FromHours(_webhookListenerOptions.TimeToLiveHr);
                var dueAt = nowMs + _webhookListenerOptions.WindowMs;

                // pull up to 100 patients whose due time <= now
                var ids = await db.SortedSetRangeByScoreAsync($"{_webhookListenerOptions.KeyPrefix}:due", double.NegativeInfinity, nowMs, take: 100);

                if (ids.Length == 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(_webhookListenerOptions.ProcessorDelayMs), stoppingToken);

                    _logger.LogTrace("No items to process.");

                    continue; 
                    
                }

                foreach (var rv in ids)
                {
                    PointClickCareKey pccKey = (string)rv!;

                    // lock so only one worker handles this pid
                    if (!await db.StringSetAsync(
                        $"{_webhookListenerOptions.KeyPrefix}:lock:{pccKey}", 
                        "1",  TimeSpan.FromSeconds(60), When.NotExists))
                    {
                        continue;
                    }

                    try
                    {
                        // double-check quiet window (handles webhooks that arrived while we were picking up)
                        var lastVal = await db.StringGetAsync($"{_webhookListenerOptions.KeyPrefix}:last:{pccKey}");
                        if (!lastVal.HasValue)
                        {
                            await db.SortedSetRemoveAsync(
                                $"{_webhookListenerOptions.KeyPrefix}:due", (string)pccKey);

                            continue;
                        }

                        var last = (long)lastVal;
                        var newDue = last + _webhookListenerOptions.WindowMs;
                        if (newDue > nowMs)
                        {
                            // someone clicked save again; push the reminder out and skip for now
                            await db.SortedSetAddAsync(
                                $"{_webhookListenerOptions.KeyPrefix}:due", (string)pccKey, newDue);

                            continue;
                        }

                        // grab what changed and process
                        var types = (await db.SetMembersAsync(
                            $"{_webhookListenerOptions.KeyPrefix}:types:{pccKey}"))
                                .Select(x => (string)x!).ToArray();

                        await db.KeyDeleteAsync(
                            $"{_webhookListenerOptions.KeyPrefix}:types:{pccKey}");

                        if (!await ProcessResident(db, pointClickCareApiService, touchpointApiService,pccKey, types))
                        {
                            await db.SortedSetAddAsync(
                                $"{_webhookListenerOptions.KeyPrefix}:error", 
                                    JsonConvert.SerializeObject(new {
                                        pccKey, types
                                    }), newDue);

                        }

                        // done; remove from the calendar
                        await db.SortedSetRemoveAsync(
                            $"{_webhookListenerOptions.KeyPrefix}:due", (string)pccKey);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("{Message} {@Error}", "Error processing webhook", ex);
                    }
                    finally
                    {
                        await db.KeyDeleteAsync(
                            $"{_webhookListenerOptions.KeyPrefix}:lock:{pccKey}");
                    }
                }
            }
        }

        private async Task<bool> ProcessResident(
            IDatabase db,
            PointClickCareApiService pointClickCareApiService,
            TouchpointApiService touchpointApiService,
            PointClickCareKey pccKey, 
            IList<string> notificationEvents)
        {

            try
            {

                var touchpointSettings = new FacilitySettings
                {
                    FacilityKey = "STJ",
                    Name = "STJ",
                    AppKey = "admin1",
                    AppKeySecret = "password",
                    AppName = "site-2"
                };

                if (notificationEvents != null)
                {
                    var pccResident = await pointClickCareApiService.GetResidentAsync(pccKey);
                    var pccObservations = await pointClickCareApiService.GetResidentObservationAsync(
                        pccKey, [ObservationTypes.Weight, ObservationTypes.Height]);

                    var touchpointPatient = pccResident.MapTouchpointResident(pccObservations);

                    bool allergySent = false;
                    List<Medication>? pccMedications = null;

                    foreach (string eventItem in notificationEvents)
                    {
                        var transactionId = DateTimeOffset.UtcNow.ToString("yyyy.MM.dd.HH.mm.ss.fffffff");
                        var eventItemPair = eventItem.Split(':');
                        var eventType = eventItemPair[0];
                        var resourceId = eventItemPair[1];
                        var eventGroup = _pointClickCareOptions.GetEventGroup(eventType);

                        await db.SetAddAsync(
                            $"{_webhookListenerOptions.KeyPrefix}:eventItem:try", $"{pccKey}:{eventGroup}:{eventItem}:{transactionId}");

                        try
                        {
                            switch (eventGroup)
                            {
                                case "ADT01":
                                case "ADT02":
                                case "ADT03":
                                case "ADT04":
                                case "ADT05":
                                case "ADT06":
                                case "ADT07":
                                    object? touchpointPayload =
                                        resourceId is null ?
                                        touchpointPatient :
                                         (await pointClickCareApiService.GetADTRecordsAsync(
                                                pccKey, [resourceId])).FirstOrDefault()?.MapTouchpointResident(pccResident, pccObservations);

                                    await touchpointApiService.Patient(
                                        touchpointSettings,
                                        eventGroup,
                                        eventType,
                                        new { patient = touchpointPayload! });

                                    break;
                                case "ALL01":
                                case "ALL02":
                                    if (allergySent)
                                    {
                                        continue;
                                    }

                                    var pccAllergies =
                                        await pointClickCareApiService.GetAllergyIntoleranceAsync(pccKey);

                                    await touchpointApiService.Allergy(
                                        touchpointSettings,
                                        eventGroup,
                                        eventType,
                                        new
                                        {
                                            patient = touchpointPatient,
                                            allergies = pccAllergies.MapTouchpointAllergy()!
                                        });

                                    allergySent = true;
                                    break;
                                case "MED01":
                                case "MED02":
                                    if (!string.IsNullOrWhiteSpace(resourceId))
                                    {

                                        pccMedications ??=
                                                await pointClickCareApiService.GetMedicationAsync(pccKey);

                                        await touchpointApiService.Medication(
                                            touchpointSettings,
                                            eventGroup,
                                            eventType,
                                            new
                                            {
                                                patient = touchpointPatient,
                                                order = pccMedications.FirstOrDefault(m => m.Id.Equals(int.Parse(resourceId)))?.MapTouchpointMedication()!
                                            });
                                    }
                                    break;
                            }
                            
                            await db.SetAddAsync(
                                $"{_webhookListenerOptions.KeyPrefix}:eventItem:success", $"{pccKey}:{eventGroup}:{eventItem}:{transactionId}");


                        }
                        catch (Exception ex)
                        {
                            await db.SetAddAsync(
                                $"{_webhookListenerOptions.KeyPrefix}:eventItem:failed", $"{pccKey}:{eventGroup}:{eventItem}::{transactionId}:{ex.Message}");

                            throw;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message} {@Error}", "Error processing resident", ex);

                return false;
            }
        }
    }
}
