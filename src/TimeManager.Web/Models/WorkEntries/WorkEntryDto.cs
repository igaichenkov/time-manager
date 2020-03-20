using System;
using TimeManager.Web.Data.WorkLog;

namespace TimeManager.Web.Models.WorkEntries
{
    public class WorkEntryDto
    {
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

            Date = workEntry.Date.ToUniversalTime().Date;
            HoursSpent = workEntry.HoursSpent;
            Notes = workEntry.Notes;
        }

        public WorkEntry ToWorkEntry(string currentUserId)
        {
            return new WorkEntry()
            {
                Date = Date,
                HoursSpent = HoursSpent,
                Notes = Notes,
                UserId = currentUserId
            };
        }
    }
}