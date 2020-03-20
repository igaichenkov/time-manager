﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TimeManager.Web.IntegrationTest.Extensions;
using TimeManager.Web.Models.Account;
using TimeManager.Web.Models.Identity;
using TimeManager.Web.Models.Responses;
using Xunit;

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
            var randomEmail = GenerateRandomEmail();

            RegisterRequest request = new RegisterRequest
            {
                Email = randomEmail,
                FirstName = FirstName,
                LastName = LastName,
                Password = TestPassword
            };

            // Act
            var responseMessage = await HttpClient.PostAsync("/api/Account/SignUp", request);

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            Assert.Contains(responseMessage.Headers, header => header.Key == "Set-Cookie" && header.Value.First().StartsWith(".AspNetCore.Identity.Application"));

            var profileResponse = await responseMessage.ReadContentAsync<ProfileResponse>();
            Assert.Equal(randomEmail, profileResponse.Email);
            Assert.Equal(FirstName, profileResponse.FirstName);
            Assert.Equal(LastName, profileResponse.LastName);

            var user = await TestServerFixture.UserManager.FindByEmailAsync(randomEmail);
            Assert.Equal(FirstName, user.FirstName);
            Assert.Equal(LastName, user.LastName);
            var signInResult = await TestServerFixture.SignInManager.CheckPasswordSignInAsync(user, TestPassword, false);
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
            var randomEmail = GenerateRandomEmail();

            RegisterRequest request = new RegisterRequest
            {
                Email = randomEmail,
                FirstName = FirstName,
                LastName = LastName,
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
                Password = TestPassword
            };

            // Act
            var responseMessage = await HttpClient.PostAsync("/api/Account/SignIn", request);

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            Assert.Contains(responseMessage.Headers, header => header.Key == "Set-Cookie" && header.Value.First().StartsWith(".AspNetCore.Identity.Application"));

            var profileResponse = await responseMessage.ReadContentAsync<ProfileResponse>();
            Assert.Equal(user.Email, profileResponse.Email);
            Assert.Equal(FirstName, profileResponse.FirstName);
            Assert.Equal(LastName, profileResponse.LastName);
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

            SignInRequest request = new SignInRequest
            {
                Email = user.Email,
                Password = TestPassword,
                RememberMe = true
            };

            // Act
            var responseMessage = await HttpClient.PostAsync("/api/Account/SignIn", request);
            Assert.True(responseMessage.IsSuccessStatusCode);

            responseMessage = await HttpClient.GetAsync("/api/Account/me");

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            var profile = await responseMessage.ReadContentAsync<ProfileResponse>();

            Assert.Equal(user.Email, profile.Email);
            Assert.Equal(user.FirstName, profile.FirstName);
            Assert.Equal(user.LastName, profile.LastName);
            Assert.Equal(PreferredHoursPerDay, profile.PreferredHoursPerDay);
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

            SignInRequest request = new SignInRequest
            {
                Email = user.Email,
                Password = TestPassword,
                RememberMe = true
            };

            var responseMessage = await HttpClient.PostAsync("/api/Account/SignIn", request);
            Assert.True(responseMessage.IsSuccessStatusCode);

            // Act
            responseMessage = await HttpClient.PostAsync("/api/Account/SignOut", null);
            Assert.True(responseMessage.IsSuccessStatusCode);

            responseMessage = await HttpClient.GetAsync("/api/Account/me");

            // Assert
            Assert.False(responseMessage.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
        }
    }
}