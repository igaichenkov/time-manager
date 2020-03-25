using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TimeManager.Web.Data.Identity;

namespace TimeManager.Web.Data
{
    public class AuthInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthInitializer(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task SeedRolesAsync()
        {
            await CreateRoleAsync(RoleNames.Admin).ConfigureAwait(false);
            await CreateRoleAsync(RoleNames.User).ConfigureAwait(false);
            await CreateRoleAsync(RoleNames.Manager).ConfigureAwait(false);
        }

        private async Task CreateRoleAsync(string roleName)
        {
            IdentityRole role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                role = new IdentityRole(roleName);
                var identityResult = await _roleManager.CreateAsync(role).ConfigureAwait(false);
                ThrowIfFailed(identityResult);
            }
        }

        private static void ThrowIfFailed(IdentityResult identityResult)
        {
            if (!identityResult.Succeeded)
            {
                throw new InvalidOperationException(identityResult.Errors
                    .FirstOrDefault()?.Description);
            }
        }
    }
}