using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TimeManager.Web.Models.Identity;
using TimeManager.Web.Models.Account;
using System.Linq;
using TimeManager.Web.Models.Responses;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Authorization;
using TimeManager.Web.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TimeManager.Web.Controllers
{
    public class AccountController : ApiControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signinManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signinManager = signinManager ?? throw new ArgumentNullException(nameof(signinManager));
        }

        [HttpPost("SignIn")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignIn([FromBody]SignInRequest singinRequest)
        {
            var user = await _signinManager.UserManager.FindByEmailAsync(singinRequest.Email);

            if (user != null)
            {
                var signInResult = await _signinManager.CheckPasswordSignInAsync(user, singinRequest.Password, true);
                if (signInResult.Succeeded)
                {
                    await _signinManager.SignInAsync(user, true);
                    return Ok(new ProfileResponse(user));
                }
            }

            return BadRequest(new ErrorResponse(new AuthenticationFailed(singinRequest.Email)));
        }

        [HttpPost("SignUp")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
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
                    return Ok(new ProfileResponse(user));
                }
            }

            return BadRequest(CreateErrorResponseFromIdentiyResult(identityResult));
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile()
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new ProfileResponse(user));
        }

        [Authorize]
        [HttpPost("SignOut")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignOut()
        {
            await _signinManager.SignOutAsync();
            return Ok();
        }

        [Authorize]
        [HttpPut("me/profile")]
        public async Task<IActionResult> PutProfile([FromBody]ChangeProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(UserId);
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
                return Ok(new ProfileResponse(user));
            }

            return BadRequest(CreateErrorResponseFromIdentiyResult(identityResult));
        }

        [Authorize]
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
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
        [HttpGet("users")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        public async Task<IEnumerable<ProfileResponse>> GetUsersList()
        {
            var users = await _userManager.Users.OrderBy(u => u.Email).ToArrayAsync().ConfigureAwait(false);
            return users.Select(user => new ProfileResponse(user));
        }

        private ErrorResponse CreateErrorResponseFromIdentiyResult(IdentityResult identityResult)
        {
            var errors = identityResult.Errors.Select(err => new ErrorDetails(err.Code, err.Description)).ToArray();
            return new ErrorResponse(errors);
        }
    }
}