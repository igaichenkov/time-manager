using System;
using TimeManager.Web.Data.WorkLog;

namespace TimeManager.Web.Models.WorkEntries
{
    public class WorkEntryDto : UpdateWorkEntryRequest
    {
        public Guid Id { get; set; }

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
        }

        public override WorkEntry ToWorkEntry(string currentUserId)
        {
            WorkEntry entry = base.ToWorkEntry(currentUserId);
            entry.Id = Id;

            return entry;
        }
    }
}