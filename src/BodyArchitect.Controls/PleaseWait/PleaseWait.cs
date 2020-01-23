using System;
using System.Diagnostics;
using System.Threading;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Common.ProgressOperation;
using BodyArchitect.Logger;
using BodyArchitect.Shared;


namespace BodyArchitect.Controls
{
    /// <summary>
    /// This class can run long operation in the background and display Please wait window to prevent user of doing anything else. 
    /// It supports canceling operations
    /// </summary>
    public static class PleaseWait
    {
        //static PleaseWaitWindow _pleaseWaitDlg;

        /// <summary>
        /// Runs the specified long operation (in delegate) in separate thread and display please wait modal window.
        /// </summary>
        /// <param name="method">The method to run in background.</param>
        /// <param name="canBeCanceled">if set to <c>true</c> operation can be canceled (PleaseWait window display Cancel button).</param>
        public static bool Run(MyRunMethod method, bool canBeCanceled=false, int? progressMax=null,bool useDefaultExceptionHandling=true)
        {
            bool res = true;
            if (useDefaultExceptionHandling)
            {
                ControlHelper.RunWithExceptionHandling(delegate
                                                           {
                                                               res = pleaseWaitImplementation(canBeCanceled, progressMax,method);
                                                           });
            }
            else
            {
                res = pleaseWaitImplementation(canBeCanceled, progressMax,method);
            }
            return res;
        }

        private static bool pleaseWaitImplementation(bool canBeCanceled, int? progressMax, MyRunMethod method)
        {
            bool res=true;
            Debug.WriteLine("PleaseWait.Run start");
            //Cancel = false;
            // Start my lengthy Process
            // Display the Please Wait Dialog here
            PleaseWaitWindow _pleaseWaitDlg = new PleaseWaitWindow(canBeCanceled, progressMax);
            Thread lengthyProcessThread = new Thread(threadRunner);
            lengthyProcessThread.IsBackground = true; // so not to have stray running threads if the main form is closed
            PleaseWaitContext pleaseWaitContext = new PleaseWaitContext(method, _pleaseWaitDlg);
            lengthyProcessThread.Start(pleaseWaitContext);
            Debug.WriteLine("Creating thread: " + lengthyProcessThread.ManagedThreadId);
            _pleaseWaitDlg.ShowDialog();
            //if please wait thread throws exception then rethrow it
            if (pleaseWaitContext.ReturnException != null)
            {
                res = false;
                ExceptionHandler.Default.Process(pleaseWaitContext.ReturnException);
                //throw new RethrowedException(pleaseWaitContext.ReturnException);)
                throw pleaseWaitContext.ReturnException;
            }
            return res;
        }

        private static void threadRunner(object obj)
        {
            ControlHelper.EnsureThreadLocalized();
            PleaseWaitContext context = (PleaseWaitContext)obj;
            PleaseWaitWindow temporaryWindow = (PleaseWaitWindow)context.ProgressWindow;
            MethodParameters par = new MethodParameters(temporaryWindow);

            temporaryWindow.Parameters = par;
            try
            {
                Debug.WriteLine("PleaseWait.Invoke before");
                context.Method.Invoke(par);
                Debug.WriteLine("PleaseWait.Invoke after");
            }
            catch (Exception ex)
            {
                context.ReturnException = ex;
            }
            temporaryWindow.ThreadSafeClose();
            Debug.WriteLine("PleaseWait.Run end");
        }
    }

    /// <summary>
    /// This delegate is used to run operation in background using PleaseWait class <see cref="PleaseWait.Run"/>
    /// </summary>
    public delegate void MyRunMethod(MethodParameters par);
}