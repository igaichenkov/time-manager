namespace TimeManager.Web.Services
{
    public interface IUserContextAccessor
    {
        string GetUserId();

        bool IsAdminUser();
    }
}