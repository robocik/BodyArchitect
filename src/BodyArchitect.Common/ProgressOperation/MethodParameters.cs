using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Common.ProgressOperation
{
    public class MethodParameters
    {
        private bool cancel;

        /// <summary>
        /// Occurs when runned operation should be canceled.
        /// </summary>
        public event EventHandler Canceled;

        private IProgressWindow pleaseWaitDlg;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodParameters"/> class.
        /// </summary>
        /// <param name="pleaseWaitDlg">The _please wait window instance.</param>
        public MethodParameters(IProgressWindow pleaseWaitDlg)
        {
            this.pleaseWaitDlg = pleaseWaitDlg;
        }

        /// <summary>
        /// You can check Cancel field to determine if user press Cancel button and stop the operation
        /// </summary>
        public bool Cancel
        {
            get
            {
                return cancel;
            }
            set
            {
                cancel = value;
                if (cancel && Canceled != null)
                {
                    Canceled(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Sets the message which will be displayed in PleaseWait window.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void SetMessage(string message)
        {
            pleaseWaitDlg.SetMessage(message);
        }

        public void SetProgress(int value)
        {
            pleaseWaitDlg.SetProgress(value);
        }

        public void CloseProgressWindow()
        {
            pleaseWaitDlg.ThreadSafeClose();
        }
    }
}
