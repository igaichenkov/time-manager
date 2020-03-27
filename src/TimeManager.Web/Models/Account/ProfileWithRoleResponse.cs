using TimeManager.Web.Models.Identity;
using TimeManager.Web.Services.Accounts;

namespace TimeManager.Web.Models.Account
{
    public class ProfileWithRoleResponse : ProfileResponse
    {
        public string RoleName { get; set; }

        public ProfileWithRoleResponse()
        {

        }

        public ProfileWithRoleResponse(UserAccount account)
            : this(account.User, account.RoleName)
        {

        }

        public ProfileWithRoleResponse(ApplicationUser user, string roleName)
            : base(user)
        {
            RoleName = roleName;
        }
    }
}
