namespace TimeManager.Web.Models.Account
{
    public class SignInRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}