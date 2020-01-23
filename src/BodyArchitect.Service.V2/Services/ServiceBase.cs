using BodyArchitect.Model;
using BodyArchitect.Service.V2.Localization;
using NHibernate;
using ProfileNotification = BodyArchitect.Model.ProfileNotification;

namespace BodyArchitect.Service.V2.Services
{
    public class MessageServiceBase : ServiceBase
    {
        private IPushNotificationService pushNotification;
        private IEMailService emailService;

        public MessageServiceBase(ISession session, SecurityInfo SecurityInfo, ServiceConfiguration configuration, IPushNotificationService pushNotification, IEMailService emailService)
            : base(session, SecurityInfo, configuration)
        {
            this.pushNotification = pushNotification;
            this.emailService = emailService;
        }


        public IPushNotificationService PushNotification
        {
            get { return pushNotification; }
        }

        public IEMailService EmailService
        {
            get { return emailService; }
        }

        //protected void SendMessage(ProfileNotification notificationType, Profile sender, Profile receiver, string messageFormat, MessageType messageType, string emailSubject, string emailMessage, params object[] args)
        //{
            
        //    if ((notificationType & ProfileNotification.Message) == ProfileNotification.Message)
        //    {
        //        MessageService messageService = new MessageService(Session, SecurityInfo, Configuration, pushNotification);
        //        messageService.SendSystemMessage(messageFormat, sender, receiver, messageType);
        //    }
        //    if ((notificationType & ProfileNotification.Email) == ProfileNotification.Email)
        //    {
        //        emailService.SendEMail(receiver, emailSubject, emailMessage, args);
        //    }
        //}

        protected void NewSendMessage(ISession session,ProfileNotification notificationType, Profile sender, Profile receiver,  string emailSubject, string emailMessage)
        {
            if (receiver.Statistics.LastLoginDate !=null &&  receiver.Statistics.LastLoginDate<Configuration.TimerService.UtcNow.AddMonths(-1))
            {//not active account (user didn't login for 1 month) so skip sending system messages
                return;
            }
            if ((notificationType & ProfileNotification.Message) == ProfileNotification.Message)
            {
                MessageService messageService = new MessageService(session, SecurityInfo, Configuration, pushNotification);
                messageService.NewSendSystemMessage(emailMessage, emailSubject, sender, receiver);
            }
            if ((notificationType & ProfileNotification.Email) == ProfileNotification.Email)
            {
                emailService.NewSendEMail(receiver, emailSubject, emailMessage);
            }
        }

        protected void NewSendMessageEx(ProfileNotification notificationType, Profile sender, Profile receiver, string keySubject, string keyMessage,params object[] args)
        {
            if (Configuration == null || pushNotification==null || (receiver.Statistics.LastLoginDate != null && receiver.Statistics.LastLoginDate < Configuration.TimerService.UtcNow.AddMonths(-1)))
            {//not active account (user didn't login for 1 month) so skip sending system messages
                return;
            }

            var culture = receiver.GetProfileCulture();
            var subject = string.Format(LocalizedStrings.ResourceManager.GetString(keySubject, culture), args);
            var message = string.Format(LocalizedStrings.ResourceManager.GetString(keyMessage, culture), args);


            if ((notificationType & ProfileNotification.Message) == ProfileNotification.Message)
            {
                MessageService messageService = new MessageService(Session, SecurityInfo, Configuration, pushNotification);
                messageService.NewSendSystemMessage(message, subject, sender, receiver);
            }
            if ((notificationType & ProfileNotification.Email) == ProfileNotification.Email)
            {
                emailService.NewSendEMail(receiver, subject, message);
            }
        }
    }
    public class ServiceBase
    {
        private ISession session;
        private ServiceConfiguration configuration;

        private SecurityInfo securityInfo;

        public ServiceBase(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
        {
            this.session = session;
            this.securityInfo = securityInfo;
            this.configuration = configuration;
        }


        

        protected ISession Session
        {
            get { return session; }
        }

        public SecurityInfo SecurityInfo
        {
            get { return securityInfo; }
        }

        public ServiceConfiguration Configuration
        {
            get { return configuration; }
        }
    }
}
