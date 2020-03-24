namespace TimeManager.Web.Models.Account
{
    public class ChangeProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float PreferredHoursPerDay { get; set; }
    }
}