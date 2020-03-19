using System;
using System.Collections.Generic;

namespace TimeManager.Web.Models.Responses
{
    public class ErrorResponse
    {
        public IReadOnlyCollection<ErrorDetails> Errors { get; set; }

        public ErrorResponse()
        {
            Errors = Array.Empty<ErrorDetails>();
        }

        public ErrorResponse(IReadOnlyCollection<ErrorDetails> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public ErrorResponse(params ErrorDetails[] errors)
            : this((IReadOnlyCollection<ErrorDetails>)errors)
        {

        }
    }

    public class ErrorDetails
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public ErrorDetails()
        {

        }

        public ErrorDetails(string code, string description)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException($"Error {nameof(code)} can't be empty", nameof(code));
            }

            Code = code;
            Description = description;
        }
    }

    public class AuthenticationFailed : ErrorDetails
    {
        private const string ErrorCode = "AUTHENTICATION_FAILED";

        public AuthenticationFailed()
            : base()
        {

        }

        public AuthenticationFailed(string userEmail)
            : base(ErrorCode, "Email or password doesn't match")
        {

        }
    }
}