using System;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace TimeManager.Web.DbErrorHandlers
{
    public class SqliteErrorHandler : IDbErrorHandler
    {
        private const int DuplicateKeyErrorCode = 19;

        public bool IsDuplicateKeyError(Exception e, params string[] fieldNames)
        {
            if (e.InnerException is SqliteException sqliteException)
            {
                if (sqliteException.SqliteErrorCode == 19)
                {
                    if (fieldNames.Length > 0)
                    {
                        return fieldNames.All(field => sqliteException.Message.Contains(field));
                    }

                    return true;
                }
            }

            return false;
        }
    }
}