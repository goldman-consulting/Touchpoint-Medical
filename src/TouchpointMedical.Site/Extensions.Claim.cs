using System.Security.Claims;

namespace TouchpointMedical.Site
{
    public static class Extensions
    {
        public static string Name(this ClaimsPrincipal user)
        {
            if (user is null || user?.Identity?.IsAuthenticated != true)
            {
                return "Anonymous";
            }

            return user.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        }

        public static string UserId(this ClaimsPrincipal user)
        {
            if (user is null || user?.Identity?.IsAuthenticated != true)
            {
                return "Anonymous";
            }

            return user.Claims.First(c => c.Type == ClaimTypes.Upn).Value;
        }

        public static string Email(this ClaimsPrincipal user)
        {
            if (user is null || user?.Identity?.IsAuthenticated != true)
            {
                return "Anonymous";
            }

            return user.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        }

        public static string Roles(this ClaimsPrincipal user)
        {
            if (user is null || user?.Identity?.IsAuthenticated != true)
            {
                return "Anonymous";
            }

            return user.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            if (user is null || user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value == "Admin";
        }

        public static string AuthenticationType(this ClaimsPrincipal user)
        {
            return user != null ? user.Identity?.AuthenticationType ?? "Dunno1" : "Dunno2";
        }

        public static bool IsAuthenticated(this ClaimsPrincipal user)
        {
            return user != null && user.Identity?.IsAuthenticated == true;
        }

    }
}
