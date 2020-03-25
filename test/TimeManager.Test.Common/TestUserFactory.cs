using System;
using TimeManager.Web.Models.Identity;

namespace TimeManager.Test.Common
{
    public static class TestUserFactory
    {
        public const string FirstName = "First";
        public const string LastName = "Last";
        public const string TestPassword = "ABCD_abcd1234$%";

        public static string GenerateRandomEmail() => Guid.NewGuid().ToString("N") + "@mail.test";

        public static ApplicationUser CreateTestUser(float preferredHoursPerDay = 0)
        {
            string randomEmail = GenerateRandomEmail();

            return new ApplicationUser()
            {
                Email = randomEmail,
                UserName = randomEmail,
                FirstName = FirstName,
                LastName = LastName,
                EmailConfirmed = true,
                PreferredHoursPerDay = preferredHoursPerDay
            };
        }
    }
}
