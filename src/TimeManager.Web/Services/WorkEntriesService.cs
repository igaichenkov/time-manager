using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeManager.Web.Data;
using TimeManager.Web.Data.WorkLog;

namespace TimeManager.Web.Services
{
    public class WorkEntriesService : IWorkEntriesService
    {
        private readonly ApplicationDbContext _dbContext;

        public WorkEntriesService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<WorkEntry> CreateAsync(WorkEntry workEntry)
        {
            _dbContext.Add(workEntry);
            await _dbContext.SaveChangesAsync();

            return workEntry;
        }

        public async Task<WorkEntry> GetByIdAsync(Guid id)
        {
            return await _dbContext.WorkEntries.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IReadOnlyCollection<WorkEntry>> FindAsync(string userId, DateTime? minDate = null, DateTime? maxDate = null)
        {
            IQueryable<WorkEntry> query = _dbContext.WorkEntries.Where(e => e.UserId == userId).OrderBy(e => e.Date);

            if (minDate.HasValue)
            {
                query = query.Where(e => e.Date >= minDate);
            }

            if (maxDate.HasValue)
            {
                query = query.Where(e => e.Date <= maxDate);
            }

            return await query.ToArrayAsync();
        }
    }
}