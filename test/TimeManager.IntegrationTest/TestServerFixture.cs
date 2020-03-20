using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using TimeManager.Web;
using TimeManager.Web.Data;
using TimeManager.Web.Models.Identity;

namespace TimeManager.Web.IntegrationTest
{
    public class TestServerFixture : WebApplicationFactory<Startup>
    {
        private const string TestDbPath = "app.db";

        public HttpClient HttpClient { get; }

        public ApplicationDbContext DbContext { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public UserManager<ApplicationUser> UserManager => ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        public SignInManager<ApplicationUser> SignInManager => ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();

        public TestServerFixture()
        {
            HttpClient = CreateClient();

            ServiceProvider = Server.Services.CreateScope().ServiceProvider;
            DbContext = ServiceProvider.GetService<ApplicationDbContext>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

                HttpClient.Dispose();
                DbContext.Dispose();

                DeleteTestDb();
            }

            base.Dispose(disposing);
        }

        private static void DeleteTestDb()
        {
            if (File.Exists(TestDbPath))
            {
                File.Delete(TestDbPath);
            }
        }
    }
}
