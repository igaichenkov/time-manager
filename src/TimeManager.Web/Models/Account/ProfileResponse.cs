using System;
using TimeManager.Web.Models.Identity;

namespace TimeManager.Web.Models.Account
{
    public class ProfileResponse : ChangeProfileRequest
    {
        public string Id { get; set; }

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

            Id = user.Id;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            PreferredHoursPerDay = user.PreferredHoursPerDay;
        }

    }
}