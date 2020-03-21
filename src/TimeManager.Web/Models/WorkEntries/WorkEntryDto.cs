using System;
using TimeManager.Web.Data.WorkLog;

namespace TimeManager.Web.Models.WorkEntries
{
    public class WorkEntryDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public float HoursSpent { get; set; }

        public string Notes { get; set; }

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

        public WorkEntry ToWorkEntry(string currentUserId)
        {
            return new WorkEntry()
            {
                Id = Id,
                Date = Date,
                HoursSpent = HoursSpent,
                Notes = Notes,
                UserId = currentUserId
            };
        }
    }
}