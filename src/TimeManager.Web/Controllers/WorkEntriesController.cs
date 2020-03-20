using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeManager.Web.Data.WorkLog;
using TimeManager.Web.Models.WorkEntries;
using TimeManager.Web.Services;

namespace TimeManager.Web.Controllers
{
    [Authorize]
    public class WorkEntriesController : ApiControllerBase
    {
        private readonly IWorkEntriesService _workEntriesService;

        public WorkEntriesController(IWorkEntriesService workEntriesService)
        {
            _workEntriesService = workEntriesService ?? throw new System.ArgumentNullException(nameof(workEntriesService));
        }

        [HttpGet]
        public async Task<WorkEntryDto[]> Get([FromQuery]DateTime? minDate = null, [FromQuery]DateTime? maxDate = null)
        {
            var entries = await _workEntriesService.FindAsync(User.Identity.Name, minDate, maxDate);
            return entries.Select(e => new WorkEntryDto(e)).ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]WorkEntryDto workEntry)
        {
            if (workEntry is null)
            {
                throw new ArgumentNullException(nameof(workEntry));
            }

            var entry = workEntry.ToWorkEntry(UserId);
            await _workEntriesService.CreateAsync(entry);

            return Ok();
        }
    }
}