using TouchpointMedical.Authorization;
using TouchpointMedical.Http.Interfaces;

namespace TouchpointMedical.Integration.PointClickCare.Authorization
{
    public record PointClickCareToken : AuthToken, IHttpContentable
    {
    }
}
