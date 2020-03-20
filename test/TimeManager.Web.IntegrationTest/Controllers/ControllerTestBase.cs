using System;
using System.Net.Http;
using System.Threading.Tasks;
using TimeManager.Web.Models.Identity;
using Xunit;

namespace TimeManager.Web.IntegrationTest.Controllers
{
    [Collection(nameof(ControllerTestCollection))]
    public class ControllerTestBase : IDisposable
    {
        protected const string FirstName = "First";
        protected const string LastName = "Last";
        protected const string TestPassword = "ABCD_abcd1234$%";

        protected const float PreferredHoursPerDay = 35;

        protected TestServerFixture TestServerFixture { get; }
        protected HttpClient HttpClient { get; }

        protected static string GenerateRandomEmail() => Guid.NewGuid().ToString("N") + "@mail.test";

        public ControllerTestBase(TestServerFixture testServerFixture)
        {
            TestServerFixture = testServerFixture;
            HttpClient = testServerFixture.CreateClient();
        }

        protected async Task<ApplicationUser> CreateTestUserAsync(float? preferredHoursPerDay = null)
        {
            string randomEmail = GenerateRandomEmail();

            var user = new ApplicationUser()
            {
                Email = randomEmail,
                UserName = randomEmail,
                FirstName = FirstName,
                LastName = LastName,
                EmailConfirmed = true,
                PreferredHoursPerDay = preferredHoursPerDay
            };

            var signInResult = await TestServerFixture.UserManager.CreateAsync(user, TestPassword).ConfigureAwait(false);
            Assert.True(signInResult.Succeeded);

            return user;
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}