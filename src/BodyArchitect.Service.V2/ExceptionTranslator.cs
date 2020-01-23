using System;
using System.Data.SqlClient;
using System.ServiceModel;
using BodyArchitect.Service.V2.Model.Exceptions;
using BodyArchitect.Shared;


namespace BodyArchitect.Service.V2
{
    public static class ExceptionTranslator
    {
        public static Exception Translate(Exception ex)
        {
            SqlException sqlCeException = ex as SqlException;
            if(sqlCeException!=null)
            {
                if (sqlCeException.Number == 2627)
                {
                    return new UniqueException("Unique exception",ex);
                }
                else
                {
                    return new DatabaseException("General database exception occurred",ex);
                }
                //((RethrowedException) ex).OriginalStackTrace = sqlCeException.StackTrace;
            }
            return ex;
        }
    }
}
