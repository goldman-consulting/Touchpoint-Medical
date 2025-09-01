using Microsoft.AspNetCore.Mvc;

using TouchpointMedical.Authorization;

namespace TouchpointMedical.Site.Controllers
{
    public partial class AuthController(IAuthorizationServiceFactory authorizationServiceFactory) : Controller
    {
        private readonly IAuthorizationServiceFactory _authorizationServiceFactory = authorizationServiceFactory;

        public async Task<IActionResult> Index(string id, string code)
        {
            var authorizationService = _authorizationServiceFactory.Get(id);

            var options = new AuthorizationActionOptions("/")
            {
                OnSuccessHandler = (authorizationToken, path) =>
                {
                    //Do something with token


                    return Redirect(path);
                }
            };

            return await authorizationService.TryAuthorize(code, options);
        }
    }
}
