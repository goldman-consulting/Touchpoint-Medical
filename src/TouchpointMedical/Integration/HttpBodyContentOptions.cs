namespace TouchpointMedical.Integration
{
    public class HttpBodyContentOptions : HttpCallOptions
    {
        public  required HttpContent PostBodyContent { get; set; }
    }
}
