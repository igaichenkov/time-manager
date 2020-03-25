using System;
using TimeManager.Web.Data.WorkLog;

namespace TimeManager.Web.Models.WorkEntries
{
    public class WorkEntryDto : UpdateWorkEntryRequest
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public WorkEntryDto()
        {

        }

        public WorkEntryDto(WorkEntry workEntry)
        {
            if (workEntry is null)
            {
                throw new ArgumentNullException(nameof(workEntry));
            }

            Id = workEntry.Id;
            Date = workEntry.Date;
            HoursSpent = workEntry.HoursSpent;
            Notes = workEntry.Notes;
            UserId = workEntry.UserId;
        }

        public override WorkEntry ToWorkEntry()
        {
            WorkEntry entry = base.ToWorkEntry();
            entry.Id = Id;
            entry.UserId = UserId;

            return entry;
        }
    }
}