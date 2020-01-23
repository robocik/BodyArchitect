using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace BodyArchitect.Logger
{
    public class ErrorEventArgs:EventArgs
    {
        private bool shouldSend;
        private bool? applyAlways;
        private Exception exception;
        private string message;
        private Guid errorId;
        private ErrorWindow errorWindow;

        public ErrorEventArgs(Exception exception, Guid errorId, string displayMessage, ErrorWindow errorWindow)
        {
            this.exception = exception;
            message = displayMessage;
            this.errorId = errorId;
            this.errorWindow = errorWindow;
        }

        public Exception Exception
        {
            get { return exception; }
        }

        public bool ShouldSend
        {
            get { return shouldSend; }
            set { shouldSend = value; }
        }

        public bool? ApplyAlways
        {
            get { return applyAlways; }
            set { applyAlways = value; }
        }

        public Guid ErrorId
        {
            get { return errorId; }
        }

        public string DisplayMessage
        {
            get { return message; }
        }

        public ErrorWindow ErrorWindow
        {
            get { return errorWindow; }
        }
    }
}
