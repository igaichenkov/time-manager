using System;

namespace TimeManager.Web.Models.Account
{
    public class ProfileResponse
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ProfileResponse()
        {

        }

        public ProfileResponse(string email, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email can't be empty", nameof(email));
            }

            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}