using TouchpointMedical.Http.Interfaces;

namespace TouchpointMedical.Auth
{
    public interface ITokenService
    {
        Task<string> GetAccessTokenAsync(ITokenServiceInstanceKey? instanceKey, CancellationToken ct);
        Task<string> RefreshTokenAsync(ITokenServiceInstanceKey? instanceKey, CancellationToken ct);
    }
}
