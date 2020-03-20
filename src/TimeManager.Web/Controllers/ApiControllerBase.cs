using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeManager.Web.ActionFilters;
using TimeManager.Web.Models.Responses;

namespace TimeManager.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ServiceFilter(typeof(ArgumentExceptionHandlerAttribute))]
    public abstract class ApiControllerBase : Controller
    {
        protected string UserId => User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}