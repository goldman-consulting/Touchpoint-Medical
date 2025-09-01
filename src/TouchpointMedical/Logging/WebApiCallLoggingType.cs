namespace TouchpointMedical.Logging
{
    [Flags]
    public enum WebApiCallLoggingType
    {
        None        = 0,
        Uri         = 1 << 0,
        Request     = 1 << 1,
        Response    = 1 << 2,
        WithHeaders = 1 << 4,

        All = Uri | Request | Response | WithHeaders
    }
}
