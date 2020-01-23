using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using BodyArchitect.Settings;

namespace BodyArchitect.Logger
{
    public enum ErrorWindow
    {
        None,
        MessageBox,
        EMailReport
    }

    public static class ExceptionHandler
    {
        static ExceptionHandlerLogic logic = new ExceptionHandlerLogic(true);

        public static ExceptionHandlerLogic Default
        {
            get
            {
                return logic;
            }
        }

        public static string GetExceptionString(this Exception exception)
        {
            StringBuilder builder = new StringBuilder();
            string inner = "EXCEPTION: {2}\r\nType: {0}\r\nStackTrace:{1}\r\n";
            Exception tmp = exception;
            while (tmp!=null)
            {
                builder.AppendFormat(inner, tmp.GetType().Name, tmp.StackTrace, tmp.Message);
                //builder.Append(string.Format(inner, exception.GetType().Name, exception.StackTrace));
                tmp = tmp.InnerException;
                inner = "INNER EXCEPTION: {2}\r\nType: {0}\r\nStackTrace:{1}\r\n";
            }
            return builder.ToString();
        }
    }


}
