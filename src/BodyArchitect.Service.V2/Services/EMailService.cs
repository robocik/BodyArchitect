using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Services
{
    class EMailService : IEMailService
    {
        public string EMailSmtpServer
        {
            get
            {
                if (WebConfigurationManager.AppSettings["EMailSmtpServer"] != null)
                {
                    return WebConfigurationManager.AppSettings["EMailSmtpServer"];
                }
                return null;
            }
        }
        public string EMailUser
        {
            get
            {
                if (WebConfigurationManager.AppSettings["EMailUser"] != null)
                {
                    return WebConfigurationManager.AppSettings["EMailUser"];
                }
                return null;
            }
        }

        public bool SendRealMessages
        {
            get
            {
                bool sendRealMessages = true;
                if (WebConfigurationManager.AppSettings["SendRealMessages"] != null)
                {
                    bool.TryParse(WebConfigurationManager.AppSettings["SendRealMessages"], out sendRealMessages);
                }

                return sendRealMessages;
            }
        }

        public bool UseSSL
        {
            get
            {
                bool res = false;
                if (WebConfigurationManager.AppSettings["UseSSL"] != null)
                {
                    bool.TryParse(WebConfigurationManager.AppSettings["UseSSL"],out res);
                }
                return res;
            }
        }

        public string EMailAccount
        {
            get
            {
                if (WebConfigurationManager.AppSettings["EMailAccount"] != null)
                {
                    return WebConfigurationManager.AppSettings["EMailAccount"];
                }
                return null;
            }
        }
        public string EMailPassword
        {
            get
            {
                if (WebConfigurationManager.AppSettings["EMailPassword"] != null)
                {
                    return WebConfigurationManager.AppSettings["EMailPassword"];
                }
                return null;
            }
        }

        public void NewSendEMail(Profile profile, string subject, string message)
        {
            try
            {
                Log.WriteVerbose("sendEmailMessage profile:{0}", profile.UserName);
                MailMessage msg = new MailMessage(EMailAccount, profile.Email);
                msg.Subject = subject;
                msg.Body = message;

                using (SmtpClient smtpClient = new SmtpClient(EMailSmtpServer))
                {
                    smtpClient.Credentials = new NetworkCredential(EMailUser, EMailPassword);
                    smtpClient.EnableSsl = UseSSL;
                    if (SendRealMessages)
                    {
                        smtpClient.Send(msg);
                    }
                    msg.Dispose();
                }
                Log.WriteVerbose("Email has been sent");

            }
            catch (ArgumentException ex)
            {
                ExceptionHandler.Default.Process(ex, subject, message);
                throw new EMailSendException("Problem with sending email");
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
                throw new EMailSendException("Problem with sending email");
            }
        }

        public void SendEMail(Profile profile, string subject, string message, params object[] args)
        {
            //set language the same as receiver of this email
            CultureInfo culture = profile.GetProfileCulture();
            subject = string.Format(LocalizedStrings.ResourceManager.GetString(subject, culture), args);
            message = string.Format(LocalizedStrings.ResourceManager.GetString(message, culture), args);
            SendEMail(profile,subject,message);
        }
    }
}
