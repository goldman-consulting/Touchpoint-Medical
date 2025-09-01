using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TouchpointMedical.Integration.PointClickCare.Configuration;
using TouchpointMedical.Integration.PointClickCare.Models;

namespace TouchpointMedical.Integration.PointClickCare.Integration
{
    public class PointClickCareApiService(
        IHttpClientFactory httpClientFactory, 
        IOptions<PointClickCareOptions> pccOptions,
        ILogger<PointClickCareApiService> logger,
        PointClickCareApiTokenService tokenService) : 
        ApiServiceBase<PointClickCareApiService>(
            httpClientFactory.CreateClient(nameof(PointClickCareApiService)), 
            tokenService, 
            logger, 
            pccOptions.Value.HttpClientOptions ?? new HttpClientOptions())
    {
        private PointClickCareOptions PointClickCareOptions => pccOptions.Value;

        public async Task<List<Organization>> GetOrganizationsAsync()
        {
            var uriBase = $"{BuildUri($"/applications/{PointClickCareOptions.AppName}/activations?pageSize={Options.DefaultPageSize}")}";

            return await GetPagedAsync<Organization>(uriBase);

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            //List<Organization> organizations = [];

            //int page = 1;
            //bool hasMore;
            //do
            //{
            //    var uri = $"{uriBase}&page={page++}";

            //    LogRequest(uri, HttpClient.DefaultRequestHeaders);

            //    var response = await HttpClient.GetAsync(uri);

            //    var rawResult = await response.Content.ReadAsStringAsync();

            //    var activationResult = JsonConvert.DeserializeObject<PagedResult<Organization>>(rawResult)!;

            //    LogResponse(uri, response.Headers, rawResult);

            //    foreach (Organization organization in activationResult.Data)
            //    {
            //        organizations.Add(organization);
            //    }
            //    hasMore = activationResult.Paging.HasMore;

            //} while (hasMore);

            //return organizations;

        }

        public async Task<List<Facility>> GetFacilitiesAsync(PointClickCareKey pccKey)
        {
            var uriBase = BuildOrgUri(pccKey.OrgId, "facs?pageSize={Options.DefaultPageSize}");

            return await GetPagedAsync<Facility>(uriBase);

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            //List<Facility> facilities = [];

            //int page = 1;
            //bool hasMore;
            //do
            //{
            //    var uri = $"{uriBase}&page={page++}&pageSize=200";

            //    LogRequest(uri, HttpClient.DefaultRequestHeaders);

            //    var response = await HttpClient.GetAsync(uri);

            //    string rawResult = await response.Content.ReadAsStringAsync()!;

            //    LogResponse(uri, response.Headers, rawResult);

            //    var facilityResult = JsonConvert.DeserializeObject<PagedResult<Facility>>(rawResult)!;

            //    foreach (Facility facility in facilityResult.Data)
            //    {
            //        facilities.Add(facility);
            //    }

            //    hasMore = facilityResult.Paging.HasMore;

            //} while (hasMore);

            //return facilities;
        }


        public async Task<List<ADTRecord>> GetADTRecordsAsync(
            PointClickCareKey pccKey, List<string> adtRecordIds)
        {
            if (adtRecordIds == null || adtRecordIds.Count == 0)
            {
                return await GetADTRecordsPagedAsync(pccKey);
            }

            return await GetADTRecordsByIdAsync(pccKey, adtRecordIds);
        }

        public async Task<List<ADTRecord>> GetADTRecordsPagedAsync(PointClickCareKey pccKey)
        {
            var uriBase = BuildOrgUri(pccKey.OrgId,
                $"adt-records?patientId={pccKey.ResidentId}&pageSize={Options.DefaultPageSize}");

            return await GetPagedAsync<ADTRecord>(uriBase);

            //Logger.LogTrace("{Message} {ResidentKey} {Url}", "Getting ADT Records", pccKey, uriBase);

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            //List<ADTRecord> adtRecords = [];

            //int page = 1;
            //bool hasMore;
            //do
            //{
            //    var uri = $"{uriBase}&page={page++}&pageSize=200";

            //    var response = await HttpClient.GetAsync(uri);

            //    string rawResult = await response.Content.ReadAsStringAsync()!;

            //    Logger.LogTrace("{Uri} {RawResult}", uri, rawResult);

            //    var adtResult = JsonConvert.DeserializeObject<PagedResult<ADTRecord>>(rawResult)!;

            //    foreach (ADTRecord adtRecord in adtResult.Data)
            //    {
            //        adtRecords.Add(adtRecord);
            //    }

            //    hasMore = adtResult.Paging.HasMore;

            //} while (hasMore);

            //return adtRecords;
        }

        public async Task<List<ADTRecord>> GetADTRecordsByIdAsync(
            PointClickCareKey pccKey, List<string> adtRecordIds)
        {
            string adtRecordIdCsv = "";
            foreach(var adtRecordId in adtRecordIds)
            {
                adtRecordIdCsv = $"{adtRecordId},";
            }

            var uri = BuildOrgUri(pccKey.OrgId,
                $"adt-records?facId={pccKey.FacilityId}&adtRecordIds={adtRecordIdCsv.Trim(',')}");

            return await GetPagedAsync<ADTRecord>(uri, true);

            //Logger.LogTrace("{Message} {ResidentKey} {Url}", "Getting ADT Records", pccKey, uri);

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            //List<ADTRecord> adtRecords = [];

            //var response = await HttpClient.GetAsync(uri);

            //string rawResult = await response.Content.ReadAsStringAsync()!;

            //Logger.LogTrace("{Uri} {RawResult}", uri, rawResult);

            //var adtResult = JsonConvert.DeserializeObject<PagedResult<ADTRecord>>(rawResult)!;

            //foreach (ADTRecord adtRecord in adtResult.Data)
            //{
            //    adtRecords.Add(adtRecord);
            //}

            //return adtRecords;
        }

        public async Task<Resident> GetResidentAsync(PointClickCareKey pccKey)
        {
            var uri = BuildOrgUri(pccKey.OrgId, $"patients/{pccKey.ResidentId}?includeOptionalFields=patientLegalMailingAddress");

            return await GetSingleAsync<Resident>(uri);

            //Logger.LogTrace("{Message} {ResidentKey} {Url}", "Getting Resident", pccKey, uri);

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            //var response = await HttpClient.GetAsync(uri);

            //string rawResult = await response.Content.ReadAsStringAsync()!;

            //Logger.LogTrace("{Uri} {RawResult}", uri, rawResult);

            //var resident = JsonConvert.DeserializeObject<Resident>(rawResult)!;

            //return resident;
        }

        public async Task<List<Observation>> GetResidentObservationAsync(PointClickCareKey pccKey, List<ObservationTypes>? types = null)
        {
            var uri = BuildOrgUri(pccKey.OrgId, $"observations?patientId={pccKey.ResidentId}&pageSize={Options.DefaultPageSize}");

            if (types != null && types.Count > 0)
            {
                Dictionary<ObservationTypes, string> observationMap = new()
                {
                    { ObservationTypes.HeartRate, "heartrate" },
                    { ObservationTypes.BloodPressure, "bloodPressure" },
                    { ObservationTypes.BloodSugar, "bloodSugar" },
                    { ObservationTypes.OxygenSaturation, "oxygenSaturation" },
                    { ObservationTypes.Weight, "weight" },
                    { ObservationTypes.Height, "height" },
                    { ObservationTypes.PainLevel, "painLevel" },
                    { ObservationTypes.Temperature, "temperature" },
                    { ObservationTypes.Respirations, "respirations" }
                };
                var observationTypeList = string.Empty;
                foreach (var observationType in types)
                {
                    observationTypeList += $"{observationMap[observationType]},";
                }

                uri += $"&type={observationTypeList.Trim(',')}";
            }

            return await GetPagedAsync<Observation>(uri);

            //Logger.LogTrace("{Message} {ResidentKey} {Url}", "Getting Resident Observations", pccKey, uriBase);

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            //List<Observation> observations = [];

            //int page = 1;
            //bool hasMore;
            //do
            //{
            //    var uri = $"{uriBase}&page={page++}&pageSize=200";

            //    var response = await HttpClient.GetAsync(uri);

            //    string rawResult = await response.Content.ReadAsStringAsync()!;

            //    Logger.LogTrace("{Uri} {RawResult}", uri, rawResult);

            //    var observationResult = JsonConvert.DeserializeObject<PagedResult<Observation>>(rawResult)!;
            //    foreach (Observation observation in observationResult.Data)
            //    {
            //        observations.Add(observation);
            //    }

            //    hasMore = observationResult.Paging.HasMore;

            //} while (hasMore);


            //return observations;
        }

        public async Task<List<Medication>> GetMedicationAsync(PointClickCareKey pccKey)
        {
            var uri = BuildOrgUri(pccKey.OrgId,
                $"medications?facId={pccKey.FacilityId}&patientId={pccKey.ResidentId}" +
                $"&includeOptionalFields=drugclass,physiciandetails" +
                $"&status=active,pending,discontinued,completed&pageSize={Options.DefaultPageSize}");

            return await GetPagedAsync<Medication>(uri);

            //Logger.LogTrace("{Message} {ResidentKey} {Url}", "Getting Resident Medications", pccKey, uriBase);

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            //List<Medication> medications = [];

            //int page = 1;
            //bool hasMore;
            //do
            //{
            //    var uri = $"{uriBase}&page={page++}&pageSize={Options.DefaultPageSize}";

            //    var response = await HttpClient.GetAsync(uri);

            //    string rawResult = await response.Content.ReadAsStringAsync()!;

            //    Logger.LogTrace("{Uri} {RawResult}", uri, rawResult);

            //    var medicationResult = JsonConvert.DeserializeObject<PagedResult<Medication>>(rawResult)!;

            //    foreach (Medication medication in medicationResult.Data)
            //    {
            //        medications.Add(medication);
            //    }

            //    hasMore = medicationResult.Paging.HasMore;

            //} while (hasMore);


            //return medications;
        }

        public async Task<List<Allergy>> GetAllergyIntoleranceAsync(PointClickCareKey pccKey)
        {
            var uri = BuildOrgUri(pccKey.OrgId, $"allergyintolerances?patientId={pccKey.ResidentId}&pageSize={Options.DefaultPageSize}");

            return await GetPagedAsync<Allergy>(uri);

            //Logger.LogTrace("{Message} {ResidentKey} {Url}", "Getting Resident Allergy Intolerances", pccKey, uriBase);

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            //List<Allergy> allergyIntolerances = [];

            //int page = 1;
            //bool hasMore;
            //do
            //{
            //    var uri = $"{uriBase}&page={page++}&pageSize={Options.DefaultPageSize}";

            //    var response = await HttpClient.GetAsync(uri);

            //    string rawResult = await response.Content.ReadAsStringAsync()!;

            //    Logger.LogTrace("{Uri} {RawResult}", uri, rawResult);

            //    var allergyResult = JsonConvert.DeserializeObject<PagedResult<Allergy>>(rawResult)!;
            //    foreach (Allergy allergy in allergyResult.Data)
            //    {
            //        allergyIntolerances.Add(allergy);
            //    }
            //    hasMore = allergyResult.Paging.HasMore;

            //} while (hasMore);

            //return allergyIntolerances;
        }

        public async Task<Dictionary<string, object>> GetWebhookSubscriptions()
        {
            var uriBase = BuildUri($"webhook-subscriptions?applicationName={PointClickCareOptions.AppName}&pageSize={Options.DefaultPageSize}");

            var webhooks = new Dictionary<string, object>();

            foreach (var status in new List<string> { "APPROVED", "PENDING", "CANCELLED", "REJECTED", "RETIRED" })
            {
                var uri = $"{uriBase}&status={status}";

                 webhooks.Add(status, await GetPagedAsync<object>(uri));

                //int page = 1;
                //bool hasMore;
                //do
                //{
                //    var uri = $"{uriBase}&page={page++}&pageSize={Options.DefaultPageSize}";

                //    var response = await HttpClient.GetAsync(uri);

                //    string rawResult = await response.Content.ReadAsStringAsync()!;

                //    Logger.LogTrace("{Uri} {RawResult}", uri, rawResult);

                //    var webhookResult = JsonConvert.DeserializeObject<PagedResult<object>>(rawResult)!;

                //    webhooks.Add(status, webhookResult.Data);

                //    hasMore = webhookResult.Paging.HasMore;

                //} while (hasMore);
            }

            return webhooks;
        }

        public async Task<WebhookSubscription?> CreateWebhookSubscriptionInstance()
        {
            var uri = BuildUri("webhook-subscriptions");

            var eventGroupList = new List<string>();
            //Iterate _pointClickCareOptions.WebhookEventGroupList

            var subscription = WebhookSubscription.Create(
                PointClickCareOptions.AppName,
                PointClickCareOptions.WebhookDomain,
                PointClickCareOptions.WebhookPort,
                PointClickCareOptions.WebhookUsername,
                PointClickCareOptions.WebhookPassword,
                eventGroupList
            );

            return await PostAsync<WebhookSubscription>(uri, subscription.AsJsonContent());

            //HttpClient.DefaultRequestHeaders.Authorization
            //    = new AuthenticationHeaderValue("Bearer", await GetAccessToken());




            //var response = await HttpClient.PostAsync(uri, postBodyContent);

            //subscription.RegistrationResponse = 
            //    await response.Content.ReadAsStringAsync()!;

            //return subscription;
        }

        //private PointClickCareToken? _token = default;
        //private async Task<string> GetAccessToken()
        //{
        //    if (_token == null || _token.NeedsRefresh)
        //    {
        //        Logger.LogInformation("Refreshing {@token}", _token);
        //        _token = await AuthorizeClientAsync();

        //    }
        //    return _token.AccessToken;
        //}

        private string BuildUri(string path)
        {
            var baseUri = PointClickCareOptions.BaseUri.TrimEnd('/');
            path = path.Trim('/');

            return $"{baseUri}/{path}";
        }

        private string BuildOrgUri(string orgId, string path)
        {
            var baseUri = PointClickCareOptions.BaseUri.TrimEnd('/');
            path = path.Trim('/');

            return $"{baseUri}/orgs/{orgId}/{path}";
        }


    }
}
