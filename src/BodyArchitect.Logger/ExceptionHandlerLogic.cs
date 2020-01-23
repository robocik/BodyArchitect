using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.Portable;
using BodyArchitect.Settings;
using System.Windows.Forms;

namespace BodyArchitect.Logger
{
    public class ExceptionHandlerLogic
    {
        //protected Queue<MailMessage> messages = new Queue<MailMessage>();
        //private object notifySyncObj = new object();
        //private Thread thread;
        //private bool sendEmailFirstUse=true;
        public event EventHandler<ErrorEventArgs> ShowEmailReportWindow;
        public event EventHandler<ErrorEventArgs> ShowMessageBoxWindow;

        public ExceptionHandlerLogic(bool emailFeaturesEnabled)
        {
            EmailFeaturesEnabled = emailFeaturesEnabled;
        }

        public bool EmailFeaturesEnabled { get; set; }

        private void onShowEmailReportWindow(ErrorEventArgs arg)
        {
            if (ShowEmailReportWindow != null)
            {
                ShowEmailReportWindow(null, arg);
            }
        }

        private void onShowMessageBoxWindow(ErrorEventArgs arg)
        {
            if (ShowMessageBoxWindow != null)
            {
                ShowMessageBoxWindow(null, arg);
            }
        }

        public Guid Process(Exception ex, params object[] args)
        {
            return Process(ex, null, ErrorWindow.None, args);
        }

        public Guid Process(Exception ex, string displayMessage, ErrorWindow errorWindow,params object[] args)
        {
            Guid errorId = Guid.NewGuid();
            Log.WriteInfo("VERSION: {0}, ERROR: ID={1}, Message={2}",Constants.Version, errorId, ex.Message);
            Log.Write(ex, errorId, args);

            try
            {
                sendByEmail(ex, errorId, (displayMessage != null ? string.Format(displayMessage, errorId) : null), errorWindow);
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }

            return errorId;
        }

        private void sendByEmail(Exception ex, Guid errorId, string displayMessage, ErrorWindow errorWindow)
        {
            if(!EmailFeaturesEnabled)
            {
                return;
            }
            //MailMessage msg = new MailMessage();
            //if (errorWindow == ErrorWindow.EMailReport)
            //{
            //    createErrorMessage(msg, ex);
            //}

            bool shouldSend=ApplicationSettings.SendExceptionsWithoutQuestion;

            if(errorWindow==ErrorWindow.None)
            {
                return;
            }

            ErrorEventArgs e = new ErrorEventArgs(ex, errorId, displayMessage, errorWindow);
            e.ShouldSend = errorWindow == ErrorWindow.EMailReport
                               ? shouldSend
                               : false;

            showErrorWindow(e);
            //msg.Dispose();
            shouldSend = e.ShouldSend;
            if (e.ApplyAlways.HasValue)
            {
                ApplicationSettings.AskForSendingException = !e.ApplyAlways.Value;
                if (e.ApplyAlways.Value)
                {
                    ApplicationSettings.SendExceptionsWithoutQuestion = shouldSend;
                }
            }
            if (!shouldSend)
            {//user doesn't want to send mail
                return;
            }
            //lock (messages)
            //{
            //    messages.Enqueue(msg);
            //}

            //lock (notifySyncObj)
            //{
            //    Monitor.Pulse(notifySyncObj);
            //}

            //try
            //{
                StartEMailThread(ex,errorId);

            //}
            //catch (Exception ex1)
            //{
            //    Log.Write(ex1, Guid.NewGuid());
            //}

            

            //Log.Write(ex, errorId,false,"email");
        }

        //private void createErrorMessage(MailMessage msg, Exception ex)
        //{
        //    string exMsg = ex.Message.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        //    msg.Subject = "EXCEPTION: " + exMsg;
        //    msg.From = new MailAddress(ApplicationSettings.MailAccount);
        //    msg.To.Add(ApplicationSettings.MailAccount);
        //    msg.Body = string.Format("Type: {0}\r\nStackTrace:{1}", ex.GetType().Name, ex.StackTrace);
        //    if (ex.InnerException != null)
        //    {
        //        msg.Body +=
        //            string.Format(
        //                "\r\nINNER EXCEPTION: {0}\r\nType: {1}\r\nStackTrace: {2}", ex.InnerException.Message,
        //                ex.InnerException.GetType().Name, ex.InnerException.StackTrace);
        //    }
        //    msg.IsBodyHtml = false;
        //}

        private void showErrorWindow(ErrorEventArgs e)
        {
            if (e.ErrorWindow == ErrorWindow.EMailReport)
            {
                if (ApplicationSettings.AskForSendingException)
                {
                    onShowEmailReportWindow(e);
                    return;
                }
            }
            //we should show message box in case when we have ErrorWindow.EMailReport but
            //AskForSendingException=false - mail is send automatically so we have to inform the user about error
            onShowMessageBoxWindow(e);
        }

        protected virtual void StartEMailThread(Exception ex,Guid errorId)
        {
            var thread = new Thread(new ThreadStart(delegate
            {
                Log.Write(ex, errorId, "email");
            }));
            thread.IsBackground = false;
            thread.Start();
        }
        //protected virtual void StartEMailThread()
        //{
        //    if (!sendEmailFirstUse /*|| !IsValidSMTP()*/)
        //    {
        //        return;
        //    }

        //    thread = new Thread(sendEmailThreadBody);
        //    thread.Name = "SendExceptionEmailThread";
        //    thread.IsBackground = false;
        //    thread.Start();
        //    sendEmailFirstUse = false;
        //}

        //private void sendEmailThreadBody()
        //{
        //   try
        //    {
        //        while (true)
        //        {
        //            MailMessage mail = null;
        //            try
        //            {
        //                do
        //                {
        //                    lock (messages)
        //                    {
        //                        if (messages.Count > 0)
        //                            mail = messages.Peek();
        //                        else
        //                        {
        //                            return;
        //                        }
        //                    }

        //                    SmtpClient smtpClient = new SmtpClient(ApplicationSettings.MailSmtp);
        //                    smtpClient.Credentials = new NetworkCredential(ApplicationSettings.MailUserName,ApplicationSettings.MailPassword);
        //                    smtpClient.EnableSsl = true;
        //                    smtpClient.Send(mail);
                            
                            
        //                    removeEmail(mail);
        //                }
        //                while (mail != null);
        //            }
        //            catch (SmtpFailedRecipientsException e1)
        //            {//The message could not be delivered to one or more of the recipients in To, CC, or BCC.
        //                Log.Write(e1, Guid.NewGuid());
        //                removeEmail(mail);
        //            }
        //            catch (SmtpFailedRecipientException e3)
        //            {
        //                Log.Write(e3, Guid.NewGuid());
        //                removeEmail(mail);
        //            }
        //            catch (SmtpException e2)
        //            {//The connection to the SMTP server failed.-or-Authentication failed.-or-The operation timed out.
        //                //this if statement is for not showing the same error message in every loop iteration when smtp settings are wrong
        //                Log.Write(e2, Guid.NewGuid());
        //            }


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Write(ex, Guid.NewGuid());
        //    }
        //    finally
        //   {
        //       sendEmailFirstUse = true;
        //        lock (notifySyncObj)
        //        {
        //            foreach (MailMessage queue in messages)
        //            {
        //                queue.Dispose();
        //            }
        //            messages.Clear();
        //        }
        //    }
        //}

        //private  void removeEmail(MailMessage mail)
        //{
        //    mail.Dispose();

        //    lock (messages)
        //    {
        //        if (messages.Count > 0)
        //        {
        //            messages.Dequeue();
        //        }
        //    }
        //}
    
    }
}
