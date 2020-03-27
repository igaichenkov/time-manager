using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeManager.Web.Data;

namespace TimeManager.Web.Services.Accounts
{
    public class AccountsService : IAccountsService
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IReadOnlyCollection<UserAccount>> GetUsersAsync()
        {
            var users = from user in _dbContext.Users
                        join ur in _dbContext.UserRoles
                            on user.Id equals ur.UserId
                        join role in _dbContext.Roles
                            on ur.RoleId equals role.Id
                        select new UserAccount
                        {
                            User = user,
                            RoleName = role.Name
                        };

            return await users.ToListAsync().ConfigureAwait(false);
        }
    }
}
