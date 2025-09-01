namespace TouchpointMedical.Site.Models.Home
{
    public sealed class LogListViewModel
    {        public string Directory { get; set; } = default!;
        public string RelativeDirectory { get; set; } = "";
        public string? Name { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<LogFileItem> Files { get; set; } = [];
    }
}
