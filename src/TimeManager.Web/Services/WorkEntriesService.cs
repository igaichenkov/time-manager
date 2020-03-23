using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeManager.Web.Data;
using TimeManager.Web.Data.WorkLog;
using TimeManager.Web.DbErrorHandlers;

namespace TimeManager.Web.Services
{
    public class WorkEntriesService : IWorkEntriesService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDbErrorHandler _dbErrorHandler;

        public WorkEntriesService(ApplicationDbContext dbContext, IDbErrorHandler dbErrorHandler)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbErrorHandler = dbErrorHandler ?? throw new ArgumentNullException(nameof(dbErrorHandler));
        }

        public async Task<WorkEntry> CreateAsync(WorkEntry workEntry)
        {
            if (workEntry.Id == Guid.Empty)
            {
                workEntry.Id = Guid.NewGuid();
            }

            try
            {
                _dbContext.Add(workEntry);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                return workEntry;
            }
            catch (DbUpdateException e)
            {
                if (_dbErrorHandler.IsDuplicateKeyError(e, "UserId", "Date"))
                {
                    throw new ArgumentException($"Duplicate entry for date {workEntry.Date.ToString("yyyy-MM-dd")}");
                }

                throw;
            }
        }

        public async Task<WorkEntry> GetByIdAsync(Guid id)
        {
            return await _dbContext.WorkEntries.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
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

            return await query.ToArrayAsync().ConfigureAwait(false);
        }

        public async Task<WorkEntry> UpdateAsync(WorkEntry entry)
        {
            if (entry is null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            var dbEntry = await _dbContext.WorkEntries.FindAsync(entry.Id).ConfigureAwait(false);
            if (dbEntry == null)
            {
                return null;
            }

            dbEntry.Date = entry.Date;
            dbEntry.Notes = entry.Notes;
            dbEntry.HoursSpent = entry.HoursSpent;

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return dbEntry;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entry = await _dbContext.WorkEntries.FindAsync(id).ConfigureAwait(false);

            if (entry != null)
            {
                _dbContext.WorkEntries.Remove(entry);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}