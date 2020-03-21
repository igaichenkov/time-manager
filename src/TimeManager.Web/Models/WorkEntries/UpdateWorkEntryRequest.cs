using System;
using TimeManager.Web.Data.WorkLog;

namespace TimeManager.Web.Models.WorkEntries
{
    public class UpdateWorkEntryRequest
    {
        public DateTime Date { get; set; }

        public float HoursSpent { get; set; }

        public string Notes { get; set; }

        public virtual WorkEntry ToWorkEntry(string currentUserId)
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