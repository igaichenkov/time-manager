using System;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using TimeManager.Web.Data.Identity;

namespace TimeManager.Web.Services
{
    public class UserContextAccessor : IUserContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;

        public string GetUserId()
        {
            return User.GetUserId();
        }

        public bool IsAdminUser() => User.IsInRole(RoleNames.Admin);

        public UserContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }


    }
}