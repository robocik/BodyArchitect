using System;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Common.ProgressOperation;

namespace BodyArchitect.Controls
{
    /// <summary>
    /// Internal class used in PleaseWait mechanism. Shouldn't be used directly.
    /// </summary>
    internal class PleaseWaitContext : ProgressOperationContext
    {
        private MyRunMethod method;

        /// <summary>
        /// Initializes a new instance of the <see cref="PleaseWaitContext"/> class.
        /// </summary>
        /// <param name="method">The method to run in background.</param>
        /// <param name="dlg">PleaseWaitWindow used in this context</param>
        public PleaseWaitContext(MyRunMethod method, PleaseWaitWindow dlg):base(dlg)
        {
            this.method = method;
        }

        /// <summary>
        /// Gets the method which will be run in background.
        /// </summary>
        /// <value>The method.</value>
        public MyRunMethod Method
        {
            get
            {
                return method;
            }
        }

    }
}