using System;
using TimeManager.Web.Models.Identity;

namespace TimeManager.Web.Models.Account
{
    public class ProfileResponse : ChangeProfileRequest
    {
        public string Email { get; set; }

        public ProfileResponse()
        {

        }

        public ProfileResponse(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            PreferredHoursPerDay = user.PreferredHoursPerDay;
        }

    }
}