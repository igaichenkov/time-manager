using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TimeManager.Web.IntegrationTest.Extensions;
using TimeManager.Web.Models.Account;
using TimeManager.Web.Models.Responses;
using Xunit;
using TimeManager.Test.Common;
using TimeManager.Web.Data.Identity;
using TimeManager.Web.Models.Identity;

namespace TimeManager.Web.IntegrationTest.Controllers
{
    public class AccountControllerTest : ControllerTestBase
    {
        public AccountControllerTest(TestServerFixture testServerFixture)
            : base(testServerFixture)
        {

        }

        [Fact]
        public async Task SignUp_CorrectInput_RegistersUser()
        {
            // Arrange
            var randomEmail = TestUserFactory.GenerateRandomEmail();

            RegisterRequest request = new RegisterRequest
            {
                Email = randomEmail,
                FirstName = TestUserFactory.FirstName,
                LastName = TestUserFactory.LastName,
                Password = TestUserFactory.TestPassword
            };

            // Act
            var responseMessage = await HttpClient.PostAsync("/api/Account/SignUp", request);

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            Assert.Contains(responseMessage.Headers, header => header.Key == "Set-Cookie" && header.Value.First().StartsWith(".AspNetCore.Identity.Application"));

            var profileResponse = await responseMessage.ReadContentAsync<ProfileResponse>();
            Assert.Equal(randomEmail, profileResponse.UserName);
            Assert.Equal(TestUserFactory.FirstName, profileResponse.FirstName);
            Assert.Equal(TestUserFactory.LastName, profileResponse.LastName);

            var user = await TestServerFixture.UserManager.FindByEmailAsync(randomEmail);
            Assert.Equal(TestUserFactory.FirstName, user.FirstName);
            Assert.Equal(TestUserFactory.LastName, user.LastName);
            var signInResult = await TestServerFixture.SignInManager.CheckPasswordSignInAsync(user, TestUserFactory.TestPassword, false);
            Assert.True(signInResult.Succeeded);
        }

        [Fact]
        public async Task SignUp_DuplicateEmail_Error()
        {
            // Arrange
            var randomEmail = TestUserFactory.GenerateRandomEmail();

            RegisterRequest request = new RegisterRequest
            {
                Email = randomEmail,
                FirstName = TestUserFactory.FirstName,
                LastName = TestUserFactory.LastName,
                Password = TestUserFactory.TestPassword
            };

            // Act
            await HttpClient.PostAsync("/api/Account/SignUp", request);
            var responseMessage = await HttpClient.PostAsync("/api/Account/SignUp", request);

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
            var randomEmail = TestUserFactory.GenerateRandomEmail();

            RegisterRequest request = new RegisterRequest
            {
                Email = randomEmail,
                FirstName = TestUserFactory.FirstName,
                LastName = TestUserFactory.LastName,
                Password = "1"
            };

            // Act
            var responseMessage = await HttpClient.PostAsync("/api/Account/SignUp", request);

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
                Password = TestUserFactory.TestPassword
            };

            // Act
            var responseMessage = await HttpClient.PostAsync("/api/Account/SignIn", request);

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            Assert.Contains(responseMessage.Headers, header => header.Key == "Set-Cookie" && header.Value.First().StartsWith(".AspNetCore.Identity.Application"));

            var profileResponse = await responseMessage.ReadContentAsync<ProfileWithRoleResponse>();
            Assert.Equal(user.Email, profileResponse.UserName);
            Assert.Equal(TestUserFactory.FirstName, profileResponse.FirstName);
            Assert.Equal(TestUserFactory.LastName, profileResponse.LastName);
            Assert.Equal(RoleNames.User, profileResponse.RoleName);
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
            var responseMessage = await HttpClient.PostAsync("/api/Account/SignIn", request);

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            ErrorResponse errorResponse = await responseMessage.ReadContentAsync<ErrorResponse>();

            Assert.Collection(errorResponse.Errors, err => Assert.Equal(AuthenticationFailed.ErrorCode, err.Code));
        }

        [Fact]
        public async Task Profile_AuthenticatedUser_ReturnsProfileData()
        {
            // Arrange
            var user = await CreateTestUserAsync(PreferredHoursPerDay);

            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            var responseMessage = await HttpClient.GetAsync("/api/Account/me");

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            var profile = await responseMessage.ReadContentAsync<ProfileWithRoleResponse>();

            Assert.Equal(user.Email, profile.UserName);
            Assert.Equal(user.FirstName, profile.FirstName);
            Assert.Equal(user.LastName, profile.LastName);
            Assert.Equal(PreferredHoursPerDay, profile.PreferredHoursPerDay);
            Assert.Equal(RoleNames.User, profile.RoleName);

        }

        [Fact]
        public async Task Profile_UnauthenticatedUser_Unauthorized()
        {
            // Act
            var responseMessage = await HttpClient.GetAsync("/api/Account/me");

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
        }

        [Fact]
        public async Task SignOut_AuthenticatedUser_RemovesCookie()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            var responseMessage = await HttpClient.PostAsync("/api/Account/SignOut", null);
            Assert.True(responseMessage.IsSuccessStatusCode);

            responseMessage = await HttpClient.GetAsync("/api/Account/me");

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
        }

        [Fact]
        public async Task PutProfile_UserExists_ReturnsOK()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);
            var request = new ChangeProfileRequest
            {
                FirstName = "New First Name",
                LastName = "New Last Name",
                PreferredHoursPerDay = 12
            };

            // Act
            var responseMessage = await HttpClient.PutAsync("/api/Account/me/Profile", request);
            Assert.True(responseMessage.IsSuccessStatusCode);

            // Assert
            responseMessage = await HttpClient.GetAsync("/api/Account/me");
            Assert.True(responseMessage.IsSuccessStatusCode);

            var profile = await responseMessage.ReadContentAsync<ProfileWithRoleResponse>();
            Assert.Equal(request.FirstName, profile.FirstName);
            Assert.Equal(request.LastName, profile.LastName);
            Assert.Equal(request.PreferredHoursPerDay, profile.PreferredHoursPerDay);
            Assert.Equal(RoleNames.User, profile.RoleName);
        }

        [Fact]
        public async Task PutProfile_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);
            var request = new ChangeProfileRequest
            {
                FirstName = "New First Name",
                LastName = "New Last Name",
                PreferredHoursPerDay = 12
            };

            TestServerFixture.DbContext.Users.Remove(user);
            await TestServerFixture.DbContext.SaveChangesAsync();

            // Act
            var responseMessage = await HttpClient.PutAsync("/api/Account/me/Profile", request);

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_UserExists_ReturnsOK()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);
            var changePasswordRequest = new ChangePasswordRequest
            {
                OldPassword = TestUserFactory.TestPassword,
                NewPassword = TestUserFactory.TestPassword + "123"
            };

            // Act
            var responseMessage = await HttpClient.PutAsync("/api/Account/me/password", changePasswordRequest);
            Assert.True(responseMessage.IsSuccessStatusCode);

            // Assert
            var signInRequest = new SignInRequest
            {
                Email = user.Email,
                Password = TestUserFactory.TestPassword
            };

            responseMessage = await HttpClient.PostAsync("/api/Account/SignIn", signInRequest);
            Assert.False(responseMessage.IsSuccessStatusCode);

            signInRequest.Password = changePasswordRequest.NewPassword;
            responseMessage = await HttpClient.PostAsync("/api/Account/SignIn", signInRequest);
            Assert.True(responseMessage.IsSuccessStatusCode);
        }

        [Fact]
        public async Task ChangePassword_WrongOldPassword_ReturnsOK()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);
            var changePasswordRequest = new ChangePasswordRequest
            {
                OldPassword = TestUserFactory.TestPassword + "1",
                NewPassword = TestUserFactory.TestPassword + "123"
            };

            // Act
            var responseMessage = await HttpClient.PutAsync("/api/Account/me/password", changePasswordRequest);

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            var errorResponse = await responseMessage.ReadContentAsync<ErrorResponse>();

            Assert.Equal("PasswordMismatch", errorResponse.Errors.First().Code);
        }

        [Fact]
        public async Task ChangePassword_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);
            var request = new ChangeProfileRequest
            {
                FirstName = "New First Name",
                LastName = "New Last Name",
                PreferredHoursPerDay = 12
            };

            TestServerFixture.DbContext.Users.Remove(user);
            await TestServerFixture.DbContext.SaveChangesAsync();

            // Act
            var responseMessage = await HttpClient.PutAsync("/api/Account/me/password", request);

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [Fact]
        public async Task GetUsersList_Manager_ReturnsUsersList()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var anotherUser = await CreateTestUserAsync();
            var manager = await CreateTestUserAsync(role: RoleNames.Manager);

            await HttpClient.AuthAs(manager.Email, TestUserFactory.TestPassword);

            // Act
            var profilesList = await HttpClient.GetAsync<ProfileWithRoleResponse[]>("/api/Account/users");

            // Assert
            AssertEqual(user, RoleNames.User, profilesList.First(p => p.Id == user.Id));
            AssertEqual(anotherUser, RoleNames.User, profilesList.First(p => p.Id == anotherUser.Id));
            AssertEqual(manager, RoleNames.Manager, profilesList.First(p => p.Id == manager.Id));
        }

        [Fact]
        public async Task GetUsersList_User_Forbidden()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            var responseMessage = await HttpClient.GetAsync("/api/Account/users");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
        }

        private static void AssertEqual(ApplicationUser expected, ProfileResponse actual)
        {
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.LastName, actual.LastName);
        }

        private static void AssertEqual(ApplicationUser expected, string expectedRoleName, ProfileWithRoleResponse actual)
        {
            AssertEqual(expected, actual);
            Assert.Equal(expectedRoleName, actual.RoleName);
        }
    }
}
