namespace TouchpointMedical.Site.Models.Home
{
    public sealed class LogFileItem
    {
        public string FileName { get; set; } = default!;
        public DateTime LastWrite { get; set; }
        public long SizeBytes { get; set; }
    }
}
