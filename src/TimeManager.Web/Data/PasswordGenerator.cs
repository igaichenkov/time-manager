using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeManager.Web.Data
{
    public class PasswordGenerator
    {
        private const string LowerCaseLetters = "abcdefghijkmnpqrstuvwxyz";
        private const string UpperCaseLetters = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string Digits = "123456789";
        private const string SpecialCharacters = "!@#$%^&*";

        public static string GeneratePassword(PasswordOptions options)
        {
            List<string> characterCategories = new List<string>() { LowerCaseLetters, UpperCaseLetters, Digits, SpecialCharacters };

            if (options.RequireDigit)
            {
                characterCategories.Add(Digits);
            }

            if (options.RequireLowercase)
            {
                characterCategories.Add(LowerCaseLetters);
            }

            if (options.RequireUppercase)
            {
                characterCategories.Add(UpperCaseLetters);
            }

            if (options.RequireNonAlphanumeric)
            {
                characterCategories.Add(SpecialCharacters);
            }

            var randomGenerator = new Random();

            StringBuilder passwordBuilder = new StringBuilder(options.RequiredLength);
            for (int i = 0; i < options.RequiredLength; i++)
            {
                int charCategory = i % characterCategories.Count;
                int charIndex = randomGenerator.Next(0, characterCategories[charCategory].Length);

                passwordBuilder.Append(characterCategories[charCategory][charIndex]);
            }

            return passwordBuilder.ToString();
        }
    }
}
