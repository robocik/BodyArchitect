using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Logger;
using BodyArchitect.Client.UI.Controls;

namespace BodyArchitect.Client.UI
{
    /*
     PleaseWait.Run(delegate(MethodParameters par)
                               {
                                   for (int i = 0; i < 20; i++)
                                   {
                                       par.SetProgress(i);
                                       if (par.Cancel)
                                       {
                                           return;
                                       }

                                       Thread.Sleep(1000);
                                       
                                   }
                                   
                               },true,20);
     */
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
        public static bool Run(MyRunMethod method, bool canBeCanceled = false,IProgressWindow progressWindow=null, int? progressMax = null, bool useDefaultExceptionHandling = true)
        {
            bool res = true;
            if (useDefaultExceptionHandling)
            {
                UIHelper.RunWithExceptionHandling(delegate
                {
                    res = pleaseWaitImplementation(canBeCanceled, progressMax, method, progressWindow);
                });
            }
            else
            {
                res = pleaseWaitImplementation(canBeCanceled, progressMax, method, progressWindow);
            }
            return res;
        }

        //static DispatcherTimer timer = new DispatcherTimer();
        private static bool pleaseWaitImplementation(bool canBeCanceled, int? progressMax, MyRunMethod method, IProgressWindow _pleaseWaitDlg)
        {
            bool res = true;
            Debug.WriteLine("PleaseWait.Run start");
            //Cancel = false;
            // Start my lengthy Process
            // Display the Please Wait Dialog here
            if (_pleaseWaitDlg == null)
            {
                //_pleaseWaitDlg = new PleaseWaitWindow(canBeCanceled, progressMax);
                _pleaseWaitDlg = MainWindow.Instance;
            }

            Thread lengthyProcessThread = new Thread(threadRunner);
            lengthyProcessThread.SetApartmentState(ApartmentState.STA);
            lengthyProcessThread.IsBackground = true; // so not to have stray running threads if the main form is closed
            PleaseWaitContext pleaseWaitContext = new PleaseWaitContext(method, _pleaseWaitDlg);
            lengthyProcessThread.Start(pleaseWaitContext);

            IProgressWindow temporaryWindow = _pleaseWaitDlg;
            MethodParameters par = new MethodParameters(temporaryWindow);
            
            //timer.Interval = TimeSpan.FromSeconds(.5);
            //timer.Tick += delegate
            //{
            //    timer.Stop();
            //    if (/*!pendingClosing &&*/ !par.Finished)
            //    {
            //        temporaryWindow.ShowProgress();
            //    }
            //};
            temporaryWindow.Parameters = par;
            //timer.Start();
            temporaryWindow.ShowProgress(canBeCanceled, progressMax);

            
            Debug.WriteLine("Creating thread: " + lengthyProcessThread.ManagedThreadId);
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
            Helper.EnsureThreadLocalized();
            PleaseWaitContext context = (PleaseWaitContext)obj;
            
            
            try
            {
                Debug.WriteLine("PleaseWait.Invoke before");
                context.Method.Invoke(context.ProgressWindow.Parameters);
                Debug.WriteLine("PleaseWait.Invoke after");
            }
            catch (Exception ex)
            {
                context.ReturnException = ex;
                //throw;
            }
            finally
            {
                //timer.Stop();
                context.ProgressWindow.Parameters.Finished = true;
                context.ProgressWindow.ThreadSafeClose();
                Debug.WriteLine("PleaseWait.Run end");
            }
            
        }
    }

    /// <summary>
    /// This delegate is used to run operation in background using PleaseWait class <see cref="PleaseWait.Run"/>
    /// </summary>
    public delegate void MyRunMethod(MethodParameters par);
}
