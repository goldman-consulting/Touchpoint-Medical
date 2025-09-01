using Newtonsoft.Json;

using System.Text;

using TouchpointMedical.Http.Interfaces;

namespace TouchpointMedical
{
    public static class HttpContentExtensions
    {
        public static HttpContent AsJsonContent(this IHttpContentable content)
        {
            return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        }

        public static HttpContent AsFormContent(this string content)
        {
            return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
        }



    }
}
