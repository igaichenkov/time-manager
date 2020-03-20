using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeManager.Web.Data.WorkLog;

namespace TimeManager.Web.Services
{
    public interface IWorkEntriesService
    {
        Task CreateAsync(WorkEntry workEntry);

        Task<IReadOnlyCollection<WorkEntry>> FindAsync(string userEmail, DateTime? minDate = null, DateTime? maxDate = null)
    }
}