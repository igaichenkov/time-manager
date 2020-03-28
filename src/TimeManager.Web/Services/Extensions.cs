using System.Security.Claims;

namespace TimeManager.Web.Services
{
    public static class Extensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
