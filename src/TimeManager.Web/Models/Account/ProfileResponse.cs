using System;
using TimeManager.Web.Models.Identity;

namespace TimeManager.Web.Models.Account
{
    public class ProfileResponse : ChangeProfileRequest
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public ProfileResponse()
        {

        }

        public ProfileResponse(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Id = user.Id;
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            PreferredHoursPerDay = user.PreferredHoursPerDay;
        }

    }
}