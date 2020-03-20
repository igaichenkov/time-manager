using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using TimeManager.Web.Models.Responses;

namespace TimeManager.Web.ActionFilters
{
    /// <summary>
    /// Handles uncaught <see cref="ArgumentException"/> exceptions and converts them
    /// to the BadRequest responses.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ArgumentExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public ArgumentExceptionHandlerAttribute(ILogger<ArgumentExceptionHandlerAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled && context.Exception is ArgumentException)
            {
                _logger.LogError(context.Exception, "Unhandled ArgumentException occured");

                // the caller will get the original exception message in the error description
                var apiErrorResponse = new ErrorResponse(
                    new InvalidRequestParameterError(context.Exception.Message));

                context.Result = new BadRequestObjectResult(apiErrorResponse);
                context.ExceptionHandled = true;
            }
        }
    }
}