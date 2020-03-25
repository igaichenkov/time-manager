using System;
using TimeManager.Web.Models.Identity;

namespace TimeManager.Web.Data.WorkLog
{
    public class WorkEntry
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime Date { get; set; }

        public float HoursSpent { get; set; }

        public string Notes { get; set; }

    }
}