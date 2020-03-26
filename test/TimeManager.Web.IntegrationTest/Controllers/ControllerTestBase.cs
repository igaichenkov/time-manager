using System;
using System.Net.Http;
using Xunit;
using TimeManager.Test.Common;
using System.Threading.Tasks;
using TimeManager.Web.Models.Identity;
using TimeManager.Web.Data.Identity;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TimeManager.Web.IntegrationTest.Controllers
{
    [Collection(nameof(ControllerTestCollection))]
    public class ControllerTestBase : IDisposable
    {
        protected const float PreferredHoursPerDay = 35;

        protected TestServerFixture TestServerFixture { get; }

        protected HttpClient HttpClient { get; }

        public async Task<ApplicationUser> CreateTestUserAsync(float preferredHoursPerDay = 0, string role = RoleNames.User)
        {
            var user = TestUserFactory.CreateTestUser(preferredHoursPerDay);

            var identityResult = await TestServerFixture.UserManager.CreateAsync(user, TestUserFactory.TestPassword).ConfigureAwait(false);
            Assert.True(identityResult.Succeeded);

            identityResult = await TestServerFixture.UserManager.AddToRoleAsync(user, role);
            Assert.True(identityResult.Succeeded);

            return user;
        }

        public ControllerTestBase(TestServerFixture testServerFixture)
        {
            TestServerFixture = testServerFixture;
            HttpClient = testServerFixture.CreateClient();
        }

        public void Dispose()
        {
            HttpClient.Dispose();

            try
            {
                CleanUpCollection(TestServerFixture.DbContext.Users);

                TestServerFixture.DbContext.SaveChanges();
            }
            catch
            {
                Console.WriteLine();
            }
        }

        private void CleanUpCollection<T>(DbSet<T> collection) where T: class
        {
            var items = collection.ToArray();

            foreach (var item in items)
            {
                collection.Remove(item);
            }
        }
    }
}