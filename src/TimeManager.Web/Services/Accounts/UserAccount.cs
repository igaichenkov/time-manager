using System;
using TimeManager.Web.Models.Identity;

namespace TimeManager.Web.Services.Accounts
{
    public class UserAccount
    {
        public ApplicationUser User { get; set; }

        public string RoleName { get; set; }

        public UserAccount()
        {

        }

        public UserAccount(ApplicationUser user, string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            User = user ?? throw new ArgumentNullException(nameof(user));
            RoleName = roleName;
        }
    }
}
