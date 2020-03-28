using System;
using TimeManager.Web.Data;
using TimeManager.Web.Data.WorkLog;

namespace TimeManager.Test.Common
{
    public static class TestWorkEntryFactory
    {
        public const string Notes = "note1, note2";
        public const float HoursSpent = 3.5f;

        public static WorkEntry CreateWorkEntry(DateTime date, string userId, ApplicationDbContext dbContext)
        {
            var entry = CreateWorkEntry(date, userId);
            dbContext.WorkEntries.Add(entry);

            return entry;
        }

        public static WorkEntry CreateWorkEntry(DateTime date, string userId)
        {
            return new WorkEntry
            {
                Date = date,
                Notes = Notes,
                HoursSpent = HoursSpent,
                UserId = userId
            };
        }
    }
}
