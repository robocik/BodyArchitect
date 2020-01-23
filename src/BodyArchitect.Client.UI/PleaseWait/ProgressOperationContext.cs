using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Client.UI
{
    // <summary>
    /// Internal class used in PleaseWait mechanism. Shouldn't be used directly.
    /// </summary>
    public class ProgressOperationContext
    {
        private IProgressWindow dlg;
        private Exception returnException;

        /// <summary>
        /// Initializes a new instance of the <see cref="PleaseWaitContext"/> class.
        /// </summary>
        /// <param name="dlg">PleaseWaitWindow used in this context</param>
        public ProgressOperationContext(IProgressWindow dlg)
        {

            this.dlg = dlg;
        }


        /// <summary>
        /// Gets or sets the return exception. This is set when background operation throws exception
        /// </summary>
        /// <value>The exception thrown by background operation.</value>
        public Exception ReturnException
        {
            get
            {
                return returnException;
            }
            set
            {
                returnException = value;
            }
        }

        /// <summary>
        /// Gets the PleaseWaitWindow used in this context
        /// </summary>
        /// <value>The please wait window.</value>
        public IProgressWindow ProgressWindow
        {
            get
            {
                return dlg;
            }
        }
    }
}
