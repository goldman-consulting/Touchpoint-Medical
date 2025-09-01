namespace TouchpointMedical.Integration
{
    public record Result
    {
        public required string Status { get; set; } = "Error";
        public required int Code { get; set; } = 0;
        public required string Message { get; set; }

        public bool IsSuccess => 
            Status.Equals("Success", StringComparison.InvariantCultureIgnoreCase);
    }
}
