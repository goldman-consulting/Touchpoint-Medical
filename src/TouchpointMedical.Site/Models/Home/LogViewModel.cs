namespace TouchpointMedical.Site.Models.Home
{
    public sealed class LogViewModel
    {
        public string FileName { get; set; } = default!;
        public string FullPath { get; set; } = default!;
        public DateTime LastWrite { get; set; }
        public long SizeBytes { get; set; }
        public int Tail { get; set; }
        public List<string> Lines { get; set; } = [];
    }
}
