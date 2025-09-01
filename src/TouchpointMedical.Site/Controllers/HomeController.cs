using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Diagnostics;

using TouchpointMedical.Integration.PointClickCare;
using TouchpointMedical.Integration.PointClickCare.Integration;
using TouchpointMedical.Integration.PointClickCare.Models;
using TouchpointMedical.Site.Models;
using TouchpointMedical.Site.Models.Home;

namespace TouchpointMedical.Site.Controllers
{
    public class HomeController(
        IWebHostEnvironment env,
        ILogger<HomeController> logger, 
        PointClickCareApiService pointClickCareApiService) : Controller
    {
        private readonly bool IsDevelopmentEnvironment = env.IsDevelopment()
                        || env.IsEnvironment("UAT-Gateway");

        private readonly ILogger<HomeController> _logger = logger;
        private readonly PointClickCareApiService _pointClickCareApiService = pointClickCareApiService;

        // Base logs directory: appsettings:Logging:Directory OR {ContentRoot}/Logs
        private readonly string _logsRoot = Path.GetFullPath(string.IsNullOrWhiteSpace("Logs")
                ? Path.Combine(env.ContentRootPath, "Logs")
                : "Logs");
        public IActionResult Index()
        {
            return Ok();
        }

        public async Task<IActionResult> Test(long? id, bool? includeFacility)
        {
            if (!IsDevelopmentEnvironment)
            {
                return new NotFoundResult();
            }

            try
            {
                var pccKey = PointClickCareKey.CreateResidentKey("59AFBC80-6E37-4461-8549-5B2F54187467", 12, id ?? 6209);

                List<Organization>? orgs = default;
                List<Facility>? facs = default;

                if (includeFacility ?? false)
                {
                    orgs = await _pointClickCareApiService.GetOrganizationsAsync();

                    facs = await _pointClickCareApiService.GetFacilitiesAsync(pccKey);
                }

                var resident = await _pointClickCareApiService.GetResidentAsync(pccKey);

                var residentObservations = await _pointClickCareApiService.GetResidentObservationAsync(pccKey, [ObservationTypes.Weight, ObservationTypes.Height]);

                List<ADTRecord>? adts = await _pointClickCareApiService.GetADTRecordsAsync(pccKey, []);
                List<Medication>? medications = await _pointClickCareApiService.GetMedicationAsync(pccKey);
                List<Allergy>? allergies = await _pointClickCareApiService.GetAllergyIntoleranceAsync(pccKey);

                return Json(new 
                {
                    orgs,
                    facs,
                    resident,
                    residentObservations,
                    adts,
                    medications,
                    allergies
                }, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message} {@Error}", "Unknown exception while running test script.", ex);

                return Json(new 
                {
                    errorMessage = ex.Message,
                    stack = ex.StackTrace
                }, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
        }

        public async Task<IActionResult> RegisterWebhook()
        {
            if (!IsDevelopmentEnvironment)
            {
                return new NotFoundResult();
            }

            try
            {


                var subscription = await _pointClickCareApiService.CreateWebhookSubscriptionInstance();

                return Json(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message} {@Error}", "Unknown exception while running register webhook script.", ex);

                return Json(new
                {
                    errorMessage = ex.Message,
                    stack = ex.StackTrace
                }, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
        }

        public async Task<IActionResult> Webhook()
        {
            if (!IsDevelopmentEnvironment)
            {
                return new NotFoundResult();
            }

            try
            {
                var subscriptions = await _pointClickCareApiService.GetWebhookSubscriptions();

                return Json(subscriptions, Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message} {@Error}", "Unknown exception while running webhook retreive script.", ex);

                return Json(new
                {
                    errorMessage = ex.Message,
                    stack = ex.StackTrace
                }, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
        }


        // GET /Home/Logs?dir=&name=&dateFrom=&dateTo=&page=1&pageSize=20
        [HttpGet]
        public IActionResult Logs(
            string? dir,
            string? name,
            DateTime? dateFrom,
            DateTime? dateTo,
            int page = 1,
            int pageSize = 20)
        {
            if (!IsDevelopmentEnvironment)
            {
                return new NotFoundResult();
            }

            try
            {

                var baseDir = GetSafeDir(dir);

                var files = Directory.Exists(baseDir)
                    ? Directory.EnumerateFiles(baseDir, "*", SearchOption.TopDirectoryOnly)
                    : [];

                if (!string.IsNullOrWhiteSpace(name))
                {
                    files = files.Where(f =>
                        Path.GetFileName(f).Contains(name, StringComparison.OrdinalIgnoreCase));
                }

                if (dateFrom.HasValue)
                {
                    files = files.Where(f => System.IO.File.GetLastWriteTime(f) >= dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    files = files.Where(f => System.IO.File.GetLastWriteTime(f) <= dateTo.Value);
                }

                var items = files.Select(f => new LogFileItem
                {
                    FileName = Path.GetFileName(f),
                    LastWrite = System.IO.File.GetLastWriteTime(f),
                    SizeBytes = new FileInfo(f).Length
                })
                    .OrderByDescending(x => x.LastWrite)
                    .ToList();

                var total = items.Count;
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 5, 200);

                var pageItems = items
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var vm = new LogListViewModel
                {
                    Directory = baseDir,
                    RelativeDirectory = GetRelativeDir(baseDir),
                    Name = name,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    Files = pageItems
                };

                return View("Logs", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message} {@Error}", "Unknown exception while running log retreive script.", ex);

                return Json(new
                {
                    errorMessage = ex.Message,
                    stack = ex.StackTrace
                }, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }

        }

        // GET /Home/ViewLog?file=app-2025-08-26.clef&dir=&tail=500&download=false
        [HttpGet]
        public IActionResult ViewLog(string file, string? dir, int tail = 10, bool download = false)
        {
            if (!IsDevelopmentEnvironment)
            {
                return new NotFoundResult();
            }

            try
            {

                if (string.IsNullOrWhiteSpace(file))
                {
                    return BadRequest("File is required.");
                }

                var baseDir = GetSafeDir(dir);
                var safeName = Path.GetFileName(file); // neutralize traversal
                var path = Path.Combine(baseDir, safeName);

                if (!System.IO.File.Exists(path))
                {
                    return NotFound();
                }

                if (download)
                {
                    return PhysicalFile(path, "application/octet-stream", safeName);
                }

                tail = Math.Clamp(tail, 10, 5000);

                var fi = new FileInfo(path);
                var lines = ReadLastLines(path, tail).ToList();

                var vm = new LogViewModel
                {
                    FileName = safeName,
                    FullPath = path,
                    LastWrite = fi.LastWriteTime,
                    SizeBytes = fi.Length,
                    Tail = tail,
                    Lines = lines
                };

                return View("ViewLog", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message} {@Error}", "Unknown exception while running log view script.", ex);

                return Json(new
                {
                    errorMessage = ex.Message,
                    stack = ex.StackTrace
                }, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }

        }

        // -------- helpers --------

        private string GetSafeDir(string? relative)
        {
            var target = string.IsNullOrWhiteSpace(relative)
                ? _logsRoot
                : Path.GetFullPath(Path.Combine(_logsRoot, relative));

            // prevent escaping the base logs directory
            if (!target.StartsWith(_logsRoot, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid log directory.");
            }

            return target;
        }

        private string GetRelativeDir(string absolute)
            => Path.GetRelativePath(_logsRoot, absolute).Replace('\\', '/');

        private static IEnumerable<string> ReadLastLines(string path, int count)
        {
            // Simple approach (OK for moderate files). For huge files,
            // consider a true "tail" reader using FileStream backwards.
            return System.IO.File.ReadLines(path).TakeLast(count).Reverse();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
