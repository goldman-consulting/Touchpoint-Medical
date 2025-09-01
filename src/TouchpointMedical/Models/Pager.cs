using Newtonsoft.Json;

namespace TouchpointMedical.Models
{
    public class Pager
    {
        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
    }
}
