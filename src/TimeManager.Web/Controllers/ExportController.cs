using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;
using TimeManager.Web.ActionFilters;
using TimeManager.Web.Models.WorkEntries;
using TimeManager.Web.Services;

namespace TimeManager.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ArgumentExceptionHandlerAttribute))]
    public class ExportController : Controller
    {
        private readonly IWorkEntriesService _workEntriesService;

        public ExportController(IWorkEntriesService workEntriesService)
        {
            _workEntriesService = workEntriesService ?? throw new ArgumentNullException(nameof(workEntriesService));
        }

        [HttpGet("users/{userId}/work-entries")]
        public async Task<IActionResult> ExportEntries([FromRoute] string userId, [FromQuery]DateTime? minDate = null, [FromQuery]DateTime? maxDate = null)
        {
            if ("me".Equals(userId, StringComparison.OrdinalIgnoreCase))
            {
                userId = User.GetUserId();
            }

            var entries = await GetList(userId, minDate, maxDate);
            Response.Headers.Add("Content-Disposition", new StringValues($"attachment; filename=\"Export_{DateTime.Now.ToFileTime()}.html\""));

            return View("Export", new ExportResultViewModel()
            {
                UserName = User.Identity.Name,
                Entries = entries,
                MaxDate = maxDate,
                MinDate = minDate
            });
        }

        private async Task<WorkEntryDto[]> GetList(string userId, DateTime? minDate, DateTime? maxDate)
        {
            var entries = await _workEntriesService.FindAsync(userId, minDate, maxDate);
            return entries.Select(e => new WorkEntryDto(e)).ToArray();
        }
    }
}
