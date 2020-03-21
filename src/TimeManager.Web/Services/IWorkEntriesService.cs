using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeManager.Web.Data.WorkLog;

namespace TimeManager.Web.Services
{
    public interface IWorkEntriesService
    {
        Task<WorkEntry> CreateAsync(WorkEntry workEntry);

        Task<WorkEntry> GetByIdAsync(Guid id);

        Task<IReadOnlyCollection<WorkEntry>> FindAsync(string userId, DateTime? minDate = null, DateTime? maxDate = null);
    }
}