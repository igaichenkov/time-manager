using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using TimeManager.Web.Data.Identity;
using TimeManager.Web.Models.Identity;

namespace TimeManager.Web.Data
{
    public class AuthInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthInitializer(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task SeedDataAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        public async Task SeedUsersAsync()
        {
            await GenerateUserAsync("Admin", RoleNames.Admin);
            await GenerateUserAsync("Manager", RoleNames.Manager);
        }

        public async Task SeedRolesAsync()
        {
            await CreateRoleAsync(RoleNames.Admin).ConfigureAwait(false);
            await CreateRoleAsync(RoleNames.User).ConfigureAwait(false);
            await CreateRoleAsync(RoleNames.Manager).ConfigureAwait(false);
        }

        private async Task<ApplicationUser> GenerateUserAsync(string userName, string roleName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("message", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("message", nameof(roleName));
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return user;
            }

            string password = GeneratePassword();

            user = new ApplicationUser()
            {
                EmailConfirmed = true,
                Email = userName + "@timemanager.internal.mail",
                UserName = userName
            };

            await _userManager.CreateAsync(user, password);
            Console.WriteLine($"Internal user created: {userName}, password: {password}");

            await _userManager.AddToRoleAsync(user, roleName);

            return user;
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

        private string GeneratePassword()
        {
            PasswordOptions options = _userManager.Options.Password;
            return PasswordGenerator.GeneratePassword(options);
        }
    }
}