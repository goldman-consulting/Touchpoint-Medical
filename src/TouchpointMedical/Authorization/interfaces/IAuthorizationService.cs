using Microsoft.AspNetCore.Mvc;

namespace TouchpointMedical.Authorization
{
    public interface IAuthorizationService
    {
        Task<IActionResult> TryAuthorize(string code, AuthorizationActionOptions authorizationActionOptions);
        string GetAuthorizationEndpoint();
        string GetAuthorizationCompletionResponseEndpoint();
    }
}
