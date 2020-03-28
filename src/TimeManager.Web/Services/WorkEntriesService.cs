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
        private readonly IUserContextAccessor _userContextAccessor;

        public WorkEntriesService(ApplicationDbContext dbContext, IDbErrorHandler dbErrorHandler,
            IUserContextAccessor userContextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbErrorHandler = dbErrorHandler ?? throw new ArgumentNullException(nameof(dbErrorHandler));
            _userContextAccessor = userContextAccessor ?? throw new ArgumentNullException(nameof(userContextAccessor));
        }

        public async Task<WorkEntry> CreateAsync(WorkEntry workEntry)
        {
            if (workEntry.HoursSpent <= 0 || workEntry.HoursSpent > 24)
            {
                throw new ArgumentOutOfRangeException(nameof(workEntry.HoursSpent));
            }

            if (workEntry.Id == Guid.Empty)
            {
                workEntry.Id = Guid.NewGuid();
            }

            if (string.IsNullOrWhiteSpace(workEntry.UserId) || !_userContextAccessor.IsAdminUser())
            {
                workEntry.UserId = _userContextAccessor.GetUserId();
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
                    throw new ArgumentException($"Duplicate entry for date {workEntry.Date:yyyy-MM-dd}");
                }

                throw;
            }
        }

        public async Task<WorkEntry> GetByIdAsync(Guid id)
        {
            var query = _dbContext.WorkEntries.Where(entry => entry.Id == id);
            return await AddUserIdFilterIfRequired(query)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<WorkEntry>> FindAsync(string userId, DateTime? minDate = null, DateTime? maxDate = null)
        {
            // only admins can access other users' work entries
            if (_userContextAccessor.GetUserId() != userId && !_userContextAccessor.IsAdminUser())
            {
                return Array.Empty<WorkEntry>();
            }

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
            if (entry.HoursSpent <= 0 || entry.HoursSpent > 24)
            {
                throw new ArgumentOutOfRangeException(nameof(entry.HoursSpent));
            }

            if (entry is null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            var dbEntry = await GetByIdAsync(entry.Id).ConfigureAwait(false);
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
            var entry = await GetByIdAsync(id).ConfigureAwait(false);

            if (entry != null)
            {
                _dbContext.WorkEntries.Remove(entry);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private IQueryable<WorkEntry> AddUserIdFilterIfRequired(IQueryable<WorkEntry> query)
        {
            if (!_userContextAccessor.IsAdminUser())
            {
                string userId = _userContextAccessor.GetUserId();
                return query.Where(entry => entry.UserId == userId);
            }

            return query;
        }
    }
}