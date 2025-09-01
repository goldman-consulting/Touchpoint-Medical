using Newtonsoft.Json;

namespace TouchpointMedical.Models
{
    public class PagedResult<DType>
    {
        [JsonProperty("data")]
        public required DType[] Data { get; set; }

        [JsonProperty("paging")]
        public required Pager Paging { get; set; }
    }
}
