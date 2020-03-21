using System;

namespace TimeManager.Web.DbErrorHandlers
{
    public interface IDbErrorHandler
    {
        bool IsDuplicateKeyError(Exception e, params string[] fieldNames);
    }
}