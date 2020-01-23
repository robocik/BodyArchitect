using System;
using System.ComponentModel;
using System.Net;
using System.ServiceModel;
using System.Windows;
using System.Windows.Navigation;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Model;
using BugSense;

namespace BodyArchitect.WP7.ViewModel
{
    public class MessageViewModel:ViewModelBase
    {
        public event EventHandler<OperationCompletedEventArgs> OperationCompleted;

        private MessageDTO message;
        public MessageViewModel(MessageDTO message)
        {
            this.message = message;
        }

        public string UserName
        {
            get
            {
                if (Message.Sender==null)
                {
                    return Message.Receiver.UserName;
                }
                return Message.Sender.UserName;
            }
        }

        public UserDTO User
        {
            get
            {
                if (Message.Sender == null)
                {
                    return Message.Receiver;
                }
                return Message.Sender;
            }
        }
        public PictureInfoDTO Picture
        {
            get
            {
                if (message.Sender==null)
                {
                    if (message.Receiver.Picture != null)
                    {
                        return message.Receiver.Picture;
                    }
                }
                else
                {
                    if (message.Sender.Picture != null)
                    {
                        return message.Sender.Picture;
                    }
                }
                
                return PictureInfoDTO.Empty;
            }
        }

        public string Topic
        {
            get { return Message.Topic; }
            set { Message.Topic = value; }
        }

        public string PriorityImage
        {
            get
            {
                if(message.Priority==MessagePriority.High)
                {
                    return "/Images/priority_high.png";
                }
                if (message.Priority == MessagePriority.System)
                {
                    return "/Images/priority_system.png";
                }
                if (message.Priority == MessagePriority.Low)
                {
                    return "/Images/priority_low.png";
                }
                return "/Images/priority_normal.png";
            }
        }
        public string Content
        {
            get { return Message.Content; }
            set { Message.Content = value; }
        }

        public DateTime Date
        {
            get { return Message.CreatedDate.ToLocalTime(); }
        }

        public bool CanReply
        {
            get { return !User.IsDeleted; }
        }
        public MessageDTO Message
        {
            get { return message; }
        }

        void onOperationCompleted(bool error)
        {
            if(OperationCompleted!=null)
            {
                OperationCompleted(this, new OperationCompletedEventArgs(error));
            }
        }

        public void Delete()
        {
            var m = new ServiceManager<AsyncCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<AsyncCompletedEventArgs> operationCompleted)
            {
                MessageOperationParam param = new MessageOperationParam();
                param.MessageId = Message.GlobalId;
                param.Operation = MessageOperationType.Delete;
                client1.MessageOperationCompleted -= operationCompleted;
                client1.MessageOperationCompleted += operationCompleted;
                client1.MessageOperationAsync(ApplicationState.Current.SessionData.Token, param);
            });

            m.OperationCompleted += (s, a) =>
            {
                FaultException<BAServiceException> faultEx = a.Error as FaultException<BAServiceException>;
                if (a.Error != null && (faultEx == null || faultEx.Detail.ErrorCode != ErrorCode.ObjectNotFound))
                {
                    onOperationCompleted(true);
                    BAMessageBox.ShowError(ApplicationStrings.MessageViewModel_ErrDeleteMessage);
                }
                else
                {
                    ApplicationState.Current.Cache.Messages.Remove(Message.GlobalId);
                    //ApplicationState.Current.ProfileInfo.Messages.Remove(Message);
                    onOperationCompleted(false);
                }
            };

            if (!m.Run())
            {
                onOperationCompleted(true);
                if(ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);    
                }
                
            }
        }

        public void Send()
        {
            if (message.Sender==null)
            {
                message.Sender = ApplicationState.Current.SessionData.Profile;
            }
            if(string.IsNullOrEmpty(message.Topic))
            {
                onOperationCompleted(true);
                BAMessageBox.ShowError(ApplicationStrings.MessageViewModel_ErrorTopicEmpty);
                return;
            }
            if (message.Sender.GlobalId != ApplicationState.Current.SessionData.Profile.GlobalId)
            {
                onOperationCompleted(true);
                BAMessageBox.ShowError("Err with sender");
                return;
            }
            
            var m = new ServiceManager<AsyncCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<AsyncCompletedEventArgs> operationCompleted)
            {

                client1.SendMessageCompleted -= operationCompleted;
                client1.SendMessageCompleted += operationCompleted;
                client1.SendMessageAsync(ApplicationState.Current.SessionData.Token, message);
            });

            m.OperationCompleted += (s, a) =>
               {
                   FaultException<ValidationFault> faultEx = a.Error as FaultException<ValidationFault>;
                   if (faultEx != null)
                   {
                       onOperationCompleted(true);
                       BAMessageBox.ShowError(faultEx.Detail.Details[0].Key + ":" + faultEx.Detail.Details[0].Message);
                       return;
                   }
                   if (a.Error != null)
                   {
                       BugSenseHandler.Instance.SendExceptionAsync(a.Error);
                       onOperationCompleted(true);
                       BAMessageBox.ShowError(ApplicationStrings.MessageViewModel_ErrorSendMessage);
                       return;
                   }
                   onOperationCompleted(false);
            };

            if (!m.Run())
            {
                onOperationCompleted(true);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
            
        }
    }
}
