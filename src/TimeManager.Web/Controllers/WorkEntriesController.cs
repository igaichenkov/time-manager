using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(typeof(WorkEntryDto[]), StatusCodes.Status200OK)]
        public async Task<WorkEntryDto[]> GetList([FromQuery]DateTime? minDate = null, [FromQuery]DateTime? maxDate = null)
        {
            var entries = await _workEntriesService.FindAsync(UserId, minDate, maxDate);
            return entries.Select(e => new WorkEntryDto(e)).ToArray();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkEntryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            var entry = await _workEntriesService.GetByIdAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            return Ok(new WorkEntryDto(entry));
        }

        [HttpPost]
        [ProducesResponseType(typeof(WorkEntryDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody]WorkEntryDto workEntry)
        {
            if (workEntry is null)
            {
                throw new ArgumentNullException(nameof(workEntry));
            }

            var entry = workEntry.ToWorkEntry(UserId);
            entry = await _workEntriesService.CreateAsync(entry);

            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, new WorkEntryDto(entry));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(WorkEntryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Put([FromRoute]Guid id, [FromBody]UpdateWorkEntryRequest request)
        {
            var entry = request.ToWorkEntry(UserId);
            entry.Id = id;

            entry = await _workEntriesService.UpdateAsync(entry);
            return entry == null
                ? (IActionResult)NotFound()
                : Ok(new WorkEntryDto(entry));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            await _workEntriesService.DeleteAsync(id);
            return Ok();
        }
    }
}