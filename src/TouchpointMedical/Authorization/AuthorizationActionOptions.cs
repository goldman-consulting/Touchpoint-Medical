using Microsoft.AspNetCore.Mvc;

namespace TouchpointMedical.Authorization
{
    public class AuthorizationActionOptions(
        string redirectPath, string? redirectOnErrorPath = null)
    {
        public string RedirectPath { get; init; } = redirectPath;
        public string? RedirectOnErrorPath { get; init; } = redirectOnErrorPath;
        public Func<IAuthorizationToken, string, IActionResult?>? OnSuccessHandler { get; init; } = null;

        public Func<string, string, IActionResult?>? OnErrorHandler { get; init; } = null;

    }
}
