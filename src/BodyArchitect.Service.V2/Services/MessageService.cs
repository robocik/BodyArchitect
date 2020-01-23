using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using MessagePriority = BodyArchitect.Model.MessagePriority;
using ObjectNotFoundException = BodyArchitect.Shared.ObjectNotFoundException;

namespace BodyArchitect.Service.V2.Services
{
    public class MessageService : ServiceBase
    {
        private IPushNotificationService pushNotificationService;

        public MessageService(ISession session,SecurityInfo securityInfo,ServiceConfiguration configuration, IPushNotificationService pushNotificationService)
            : base(session, securityInfo, configuration)
        {
            this.pushNotificationService = pushNotificationService;
        }

        //public  void SendSystemMessage(string message, Profile sender, Profile receiver, MessageType type)
        //{
        //    Log.WriteInfo("sendSystemMessage. Sender:{0}, Receiver:{1},Type:{2}", sender.UserName, receiver.UserName, type);
        //    var session = Session;
        //    Message msg = new Message();
        //    msg.Sender = sender;
        //    msg.Receiver = receiver;
        //    msg.Content = message;
        //    msg.MessageType = type;
        //    msg.CreatedDate = DateTime.UtcNow;
        //    msg.Priority = MessagePriority.System;
        //    session.Save(msg);

        //    pushNotificationService.SendLiveTile(session, receiver, true);
        //}

        public void NewSendSystemMessage(string message,string subject, Profile sender, Profile receiver)
        {
            Log.WriteInfo("sendSystemMessage. Sender:{0}, Receiver:{1}", sender.UserName, receiver.UserName);
            var session = Session;
            Message msg = new Message();
            msg.Sender = sender;
            msg.Receiver = receiver;
            msg.Content = message;
            msg.Topic = subject;
            msg.CreatedDate = DateTime.UtcNow;
            msg.Priority = MessagePriority.System;
            session.Save(msg);

            receiver.DataInfo.MessageHash = Guid.NewGuid();
            pushNotificationService.SendLiveTile(session, receiver, true);
        }

        public void MessageOperation( MessageOperationParam arg)
        {
            Log.WriteWarning("GetProfileInformation.Username:{0},Operation:{1},Message:{2}", SecurityInfo.SessionData.Profile.UserName, arg.Operation, arg.MessageId);

            var session = Session;

            using (var tx = session.BeginSaveTransaction())
            {
                var userDb = session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var messageDb = session.Get<Message>(arg.MessageId);
                if (messageDb == null)
                {
                    throw new ObjectNotFoundException("Message doesn't exist");
                }
                if (messageDb.Receiver != userDb)
                {
                    throw new CrossProfileOperationException("You cannot perform any operation with messages for another user");
                }
                if (arg.Operation == MessageOperationType.Delete)
                {
                    session.Delete(messageDb);
                }
                userDb.DataInfo.MessageHash = Guid.NewGuid();
                tx.Commit();
                Log.WriteVerbose("Operation completed");
                pushNotificationService.SendLiveTile(session, userDb, false);
            }

        }

        public void SendMessage( MessageDTO message)
        {
            Log.WriteWarning("SendMessage:Username={0}", SecurityInfo.SessionData.Profile.UserName);

            if (message.Sender == null)
            {
                throw new ArgumentNullException("Sender", "Sender cannot be null");
            }
            if (message.Sender.GlobalId != SecurityInfo.SessionData.Profile.GlobalId)
            {
                throw new CrossProfileOperationException("Cannot send a message from another profile");
            }
            if (message.Receiver == null)
            {
                throw new ArgumentNullException("Recievier", "Receivier cannot be null");
            }
            if (message.Priority == Model.MessagePriority.System)
            {
                throw new InvalidOperationException("You cannot send message with System priority");
            }
            Log.WriteInfo("Sender:{0}, Receiver:{1}", message.Sender.UserName, message.Receiver.UserName);
            var messageDb = message.Map<Message>();
            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                messageDb.Receiver = session.Get<Profile>(message.Receiver.GlobalId);
                if (messageDb.Receiver.IsDeleted)
                {
                    throw new UserDeletedException("Cannot send the message to the deleted profile");
                }
                messageDb.Sender = session.Get<Profile>(message.Sender.GlobalId);
                messageDb.CreatedDate = Configuration.TimerService.UtcNow;
                session.SaveOrUpdate(messageDb);

                messageDb.Receiver.DataInfo.MessageHash = Guid.NewGuid();
                tx.Commit();
            }
            pushNotificationService.SendLiveTile(session, messageDb.Receiver, true);
            Log.WriteVerbose("Message sent");
        }

        public PagedResult<MessageDTO> GetMessages(GetMessagesCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetMessages:Username={0}", SecurityInfo.SessionData.Profile.UserName);
            var session = Session;


            using (var transactionScope = session.BeginGetTransaction())
            {
                var myProfile = session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                var query = session.QueryOver<Message>().Where(x=>x.Receiver==myProfile).Fetch(x=>x.Sender).Eager;
                if(searchCriteria.FromDate.HasValue)
                {
                    query = query.Where(x => x.CreatedDate >= searchCriteria.FromDate.Value);
                }
                if (searchCriteria.ToDate.HasValue)
                {
                    query = query.Where(x => x.CreatedDate <= searchCriteria.ToDate.Value);
                }

                if(searchCriteria.SortAscending)
                {
                    query = query.OrderBy(x => x.CreatedDate).Asc;
                }
                else
                {
                    query = query.OrderBy(x => x.CreatedDate).Desc;
                }
                var listPack = query.ToPagedResults<MessageDTO, Message>(retrievingInfo);
                transactionScope.Commit();
                return listPack;
            }
        }
    }
}
