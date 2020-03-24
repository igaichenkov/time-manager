using Microsoft.AspNetCore.Identity;

namespace TimeManager.Web.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public float PreferredHoursPerDay { get; set; }
    }
}
