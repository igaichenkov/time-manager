using TimeManager.Web.Models.Identity;

namespace TimeManager.Web.Models.Account
{
    public class ProfileWithRoleResponse : ProfileResponse
    {
        public string RoleName { get; set; }

        public ProfileWithRoleResponse()
        {

        }

        public ProfileWithRoleResponse(ApplicationUser user, string roleName)
            : base(user)
        {
            RoleName = roleName;
        }
    }
}
