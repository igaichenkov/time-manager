using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TimeManager.IntegrationTest.Extensions;
using TimeManager.Web.Models.Account;
using TimeManager.Web.Models.Identity;
using TimeManager.Web.Models.Responses;
using Xunit;

namespace TimeManager.IntegrationTest.Controllers
{
    [Collection(nameof(ControllerTestCollection))]
    public class AccountControllerTest
    {
        private const string FirstName = "First";
        private const string LastName = "Last";
        private const string TestPassword = "ABCD_abcd1234$%";

        private readonly TestServerFixture _testServerFixture;

        public AccountControllerTest(TestServerFixture testServerFixture)
        {
            _testServerFixture = testServerFixture;
        }

        private static string GenerateRandomEmail() => Guid.NewGuid().ToString("N") + "@mail.test";

        [Fact]
        public async Task SignUp_CorrectInput_RegistersUser()
        {
            // Arrange
            var randomEmail = GenerateRandomEmail();

            RegisterRequest request = new RegisterRequest
            {
                Email = randomEmail,
                FirstName = FirstName,
                LastName = LastName,
                Password = TestPassword
            };

            // Act
            var responseMessage = await _testServerFixture.HttpClient.PostAsync("/api/Account/SignUp", request);

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            Assert.Contains(responseMessage.Headers, header => header.Key == "Set-Cookie" && header.Value.First().StartsWith(".AspNetCore.Identity.Application"));

            var user = await _testServerFixture.UserManager.FindByEmailAsync(randomEmail);
            Assert.Equal(FirstName, user.FirstName);
            Assert.Equal(LastName, user.LastName);
            var signInResult = await _testServerFixture.SignInManager.CheckPasswordSignInAsync(user, TestPassword, false);
            Assert.True(signInResult.Succeeded);
        }

        [Fact]
        public async Task SignUp_DuplicateEmail_Error()
        {
            // Arrange
            var randomEmail = GenerateRandomEmail();

            RegisterRequest request = new RegisterRequest
            {
                Email = randomEmail,
                FirstName = FirstName,
                LastName = LastName,
                Password = TestPassword
            };

            // Act
            await _testServerFixture.HttpClient.PostAsync("/api/Account/SignUp", request);
            var responseMessage = await _testServerFixture.HttpClient.PostAsync("/api/Account/SignUp", request);

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            ErrorResponse errorResponse = await responseMessage.ReadContentAsync<ErrorResponse>();
            Assert.Single(errorResponse.Errors);

            Assert.Equal("DuplicateUserName", errorResponse.Errors.First().Code);
            Assert.Contains(randomEmail, errorResponse.Errors.First().Description);
        }

        [Fact]
        public async Task SignUp_TooSimplePassword_Error()
        {
            // Arrange
            var randomEmail = GenerateRandomEmail();

            RegisterRequest request = new RegisterRequest
            {
                Email = randomEmail,
                FirstName = FirstName,
                LastName = LastName,
                Password = "1"
            };

            // Act
            var responseMessage = await _testServerFixture.HttpClient.PostAsync("/api/Account/SignUp", request);

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            ErrorResponse errorResponse = await responseMessage.ReadContentAsync<ErrorResponse>();
            Assert.Contains(errorResponse.Errors, err => err.Code == "PasswordTooShort");
        }

        [Fact]
        public async Task SignIn_CorrectCredentials_SetsCookie()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            SignInRequest request = new SignInRequest
            {
                Email = user.Email,
                Password = TestPassword
            };

            // Act
            var responseMessage = await _testServerFixture.HttpClient.PostAsync("/api/Account/SignIn", request);

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            Assert.Contains(responseMessage.Headers, header => header.Key == "Set-Cookie" && header.Value.First().StartsWith(".AspNetCore.Identity.Application"));
        }

        [Fact]
        public async Task SignIn_InvalidCredentials_Error()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            SignInRequest request = new SignInRequest
            {
                Email = user.Email,
                Password = "1"
            };

            // Act
            var responseMessage = await _testServerFixture.HttpClient.PostAsync("/api/Account/SignIn", request);

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            ErrorResponse errorResponse = await responseMessage.ReadContentAsync<ErrorResponse>();

            Assert.Collection(errorResponse.Errors, err => Assert.Equal(AuthenticationFailed.ErrorCode, err.Code));
        }

        [Fact]
        public async Task Profile_AuthenticatedUser_ReturnsProfileData()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            SignInRequest request = new SignInRequest
            {
                Email = user.Email,
                Password = TestPassword,
                RememberMe = true
            };

            // Act
            var responseMessage = await _testServerFixture.HttpClient.PostAsync("/api/Account/SignIn", request);
            Assert.True(responseMessage.IsSuccessStatusCode);

            responseMessage = await _testServerFixture.HttpClient.GetAsync("/api/Account/me");

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            var profile = await responseMessage.ReadContentAsync<ProfileResponse>();

            Assert.Equal(user.Email, profile.Email);
            Assert.Equal(user.FirstName, profile.FirstName);
            Assert.Equal(user.LastName, profile.LastName);
        }

        [Fact]
        public async Task Profile_UnauthenticatedUser_Unauthorized()
        {
            // Act
            var responseMessage = await _testServerFixture.HttpClient.GetAsync("/api/Account/me");

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
        }

        private async Task<ApplicationUser> CreateTestUserAsync()
        {
            string randomEmail = GenerateRandomEmail();

            var user = new ApplicationUser()
            {
                Email = randomEmail,
                UserName = randomEmail,
                FirstName = FirstName,
                LastName = LastName,
                EmailConfirmed = true
            };

            var signInResult = await _testServerFixture.UserManager.CreateAsync(user, TestPassword).ConfigureAwait(false);
            Assert.True(signInResult.Succeeded);

            return user;
        }
    }
}
