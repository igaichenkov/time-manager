using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeManager.Web.Services.Accounts
{
    public interface IAccountsService
    {
        Task<IReadOnlyCollection<UserAccount>> GetUsersAsync();
    }
}