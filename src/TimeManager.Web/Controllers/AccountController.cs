using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeManager.Web.Data.Identity;
using TimeManager.Web.Models.Account;
using TimeManager.Web.Models.Identity;
using TimeManager.Web.Models.Responses;
using TimeManager.Web.Services.Accounts;

namespace TimeManager.Web.Controllers
{
    public class AccountController : ApiControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly IAccountsService _accountsService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signinManager,
            IAccountsService accountsService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signinManager = signinManager ?? throw new ArgumentNullException(nameof(signinManager));
            _accountsService = accountsService ?? throw new ArgumentNullException(nameof(accountsService));
        }

        [HttpPost("SignIn")]
        [ProducesResponseType(typeof(ProfileWithRoleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignIn([FromBody]SignInRequest singinRequest)
        {
            var user = await _signinManager.UserManager.FindByNameAsync(singinRequest.UserName);

            if (user != null)
            {
                var signInResult = await _signinManager.CheckPasswordSignInAsync(user, singinRequest.Password, true);
                if (signInResult.Succeeded)
                {
                    await _signinManager.SignInAsync(user, true);
                    return Ok(await CreateProfileResponse(user));
                }
            }

            return BadRequest(new ErrorResponse(new AuthenticationFailed(singinRequest.UserName)));
        }

        [HttpPost("SignUp")]
        [ProducesResponseType(typeof(ProfileWithRoleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignUp([FromBody]RegisterRequest registerRequest)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                EmailConfirmed = true
            };

            IdentityResult identityResult = await _userManager.CreateAsync(user, registerRequest.Password);
            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddToRoleAsync(user, RoleNames.User);

                if (identityResult.Succeeded)
                {
                    await _signinManager.SignInAsync(user, false);
                    return Ok(await CreateProfileResponse(user));
                }
            }

            return BadRequest(CreateErrorResponseFromIdentiyResult(identityResult));
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ProfileWithRoleResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> GetProfile()
        {
            return GetProfile(UserId);
        }

        [Authorize(Policy = AuthPolicies.ManageUsers)]
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(ProfileWithRoleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile([FromRoute]string userId)
        {
            
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(await CreateProfileResponse(user));
        }

        [Authorize]
        [HttpPost("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await _signinManager.SignOutAsync();
            return Ok();
        }

        [Authorize]
        [HttpPut("me")]
        [ProducesResponseType(typeof(ProfileWithRoleResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> PutProfile([FromBody]ChangeProfileRequest request)
        {
            return PutProfile(UserId, request);
        }

        [Authorize(Policy = AuthPolicies.ManageUsers)]
        [HttpPut("users/{userId}")]
        [ProducesResponseType(typeof(ProfileWithRoleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutProfile([FromRoute]string userId, [FromBody]ChangeProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PreferredHoursPerDay = request.PreferredHoursPerDay;

            IdentityResult identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded)
            {
                return Ok(await CreateProfileResponse(user));
            }

            return BadRequest(CreateErrorResponseFromIdentiyResult(identityResult));
        }

        [Authorize]
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }
            
            var identityResult = await _userManager.ChangePasswordAsync(user, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            if (identityResult.Succeeded)
            {
                return Ok();
            }

            return BadRequest(CreateErrorResponseFromIdentiyResult(identityResult));
        }

        [Authorize(Policy = AuthPolicies.ManageUsers)]
        [HttpPut("users/{userId}/password")]
        public async Task<IActionResult> ChangePassword([FromRoute]string userId, [FromBody]ResetPasswordRequest changePasswordRequest)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var identityResult = await _userManager.ResetPasswordAsync(user, resetToken, changePasswordRequest.NewPassword);

            if (identityResult.Succeeded)
            {
                return Ok();
            }

            return BadRequest(CreateErrorResponseFromIdentiyResult(identityResult));
        }

        [Authorize(Policy = AuthPolicies.ManageUsers)]
        [HttpGet("users")]
        [ProducesResponseType(typeof(ProfileWithRoleResponse[]), StatusCodes.Status200OK)]
        public async Task<IEnumerable<ProfileWithRoleResponse>> GetUsersList()
        {
            var accounts = await _accountsService.GetUsersAsync().ConfigureAwait(false);
            return accounts.Select(user => new ProfileWithRoleResponse(user));
        }

        private ErrorResponse CreateErrorResponseFromIdentiyResult(IdentityResult identityResult)
        {
            var errors = identityResult.Errors.Select(err => new ErrorDetails(err.Code, err.Description)).ToArray();
            return new ErrorResponse(errors);
        }

        private async Task<ProfileWithRoleResponse> CreateProfileResponse(ApplicationUser user)
        {
            var roles = await _signinManager.UserManager.GetRolesAsync(user);
            return new ProfileWithRoleResponse(user, roles.FirstOrDefault());
        }
    }
}