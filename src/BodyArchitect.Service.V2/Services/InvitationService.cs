using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using ProfileNotification = BodyArchitect.Model.ProfileNotification;

namespace BodyArchitect.Service.V2.Services
{
    class InvitationService: MessageServiceBase
    {

        public InvitationService(ISession session, SecurityInfo SecurityInfo, ServiceConfiguration configuration, IPushNotificationService pushNotification, IEMailService emailService)
            : base(session, SecurityInfo, configuration, pushNotification,emailService)
        {
            
        }


        void acceptInvitation(ISession session, Profile inviter, Profile invited, FriendInvitation existingInvitation, InviteFriendOperationData data)
        {
            Log.WriteVerbose("accept invitation. Friend:{0}, Inviter:{1}", inviter.UserName, invited.UserName);
            inviter.Friends.Add(invited);
            invited.Friends.Add(inviter);
            session.Update(inviter);
            session.Update(invited);
            session.Delete(existingInvitation);

            //now check and remove this user from followers list
            invited.FavoriteUsers.Remove(inviter);
            inviter.FavoriteUsers.Remove(invited);
            ProfileStatisticsUpdater.UpdateFriends(session, invited, inviter);
            //MessageService messageService = new MessageService(Session, null, Configuration, PushNotification);
            //messageService.SendSystemMessage(data.Message,invited, inviter, MessageType.InvitationAccepted);
            //SendMessage(inviter.Settings.NotificationSocial, invited, inviter, data.Message, MessageType.InvitationAccepted, "AcceptInvitationEMailSubject", "AcceptInvitationEMailMessage", inviter.UserName,DateTime.Now,data.Message);

            NewSendMessageEx(inviter.Settings.NotificationSocial, invited, inviter, "AcceptInvitationEMailSubject", "AcceptInvitationEMailMessage", inviter.UserName, DateTime.Now, data.Message);
        }

        public FriendInvitationDTO InviteFriendOperation(InviteFriendOperationData data)
        {
            Log.WriteWarning("InviteFriendOperation:Username={0},Operation:{1},User:{2}", SecurityInfo.SessionData.Profile.UserName, data.Operation, data.User.UserName);

            if (SecurityInfo.SessionData.Profile.GlobalId == data.User.GlobalId)
            {
                throw new InvalidOperationException("You cannot add yourself as a friend");
            }
            var session = Session;

            FriendInvitation returnValue = null;
            using (var tx = session.BeginSaveTransaction())
            {
                var friendProfile = session.Get<Profile>(data.User.GlobalId);
                var inviter = session.Get<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                if (friendProfile.IsDeleted)
                {
                    throw new UserDeletedException("Cannot invite a deleted profile");
                }
                if (data.Operation == Model.InviteFriendOperation.Invite)
                {
                    if (inviter.Friends.Contains(friendProfile))
                    {
                        throw new ProfileAlreadyFriendException("Specified profile is already a friend");
                    }
                    //check if user add invitation second time
                    var existingInvitation = session.Get<FriendInvitation>(new FriendInvitation()
                    {
                        Inviter = inviter,
                        Invited = friendProfile
                    });
                    if (existingInvitation == null)
                    {
                        Log.WriteVerbose("1: existingInvitation == null");
                        existingInvitation = session.Get<FriendInvitation>(new FriendInvitation()
                        {
                            Inviter = friendProfile,
                            Invited = inviter
                        });
                        if (existingInvitation == null)
                        {
                            Log.WriteVerbose("2: existingInvitation == null. Perform invite operation");
                            FriendInvitation invitation = new FriendInvitation();
                            invitation.Inviter = inviter;
                            invitation.Invited = friendProfile;
                            invitation.Message = data.Message;
                            invitation.CreateDate = Configuration.TimerService.UtcNow;
                            invitation.InvitationType = FriendInvitationType.Invite;
                            session.Save(invitation);
                            returnValue = invitation;

                            if ((friendProfile.Settings.NotificationSocial & ProfileNotification.Email) == ProfileNotification.Email)
                            {
                                EmailService.SendEMail(friendProfile, "InviteFriendEMailSubject", "InviteFriendEMailMessage",inviter.UserName, DateTime.Now, data.Message);
                            }

                        }
                        else
                        {
                            acceptInvitation(session, friendProfile, inviter, existingInvitation, data);
                        }
                    }
                }
                else if (data.Operation == Model.InviteFriendOperation.Accept)
                {
                    var existingInvitation = session.Get<FriendInvitation>(new FriendInvitation()
                    {
                        Inviter = friendProfile,
                        Invited = inviter
                    });
                    if (existingInvitation != null)
                    {
                        acceptInvitation(session, friendProfile, inviter, existingInvitation, data);
                    }
                    else
                    {
                        throw new CannotAcceptRejectInvitationDoesntExistException("You are not invited to be a friend for specified user");
                    }
                }
                else
                {//reject
                    var existingInvitation = session.Get<FriendInvitation>(new FriendInvitation()
                    {
                        Inviter = friendProfile,
                        Invited = inviter
                    });

                    if (existingInvitation != null)
                    {
                        Log.WriteVerbose("3: existingInvitation != null.");
                        if (existingInvitation.InvitationType == FriendInvitationType.Invite)
                        {
                            session.Delete(existingInvitation);

                            //messageService.SendSystemMessage(data.Message, inviter, friendProfile, MessageType.InvitationRejected);
                            //SendMessage(friendProfile.Settings.NotificationSocial, inviter, friendProfile, data.Message, MessageType.InvitationRejected
                            //, "RejectInvitationEmailSubject", "RejectInvitationEmailMessage", inviter.UserName, DateTime.Now, data.Message);
                            NewSendMessageEx(inviter.Settings.NotificationSocial, inviter, friendProfile, "RejectInvitationEmailSubject", "RejectInvitationEmailMessage", inviter.UserName, DateTime.Now, data.Message);
                        }
                        else
                        {
                            Log.WriteError("4:This should not be invoked.");
                        }

                    }
                    else if (inviter.Friends.Contains(friendProfile))
                    {
                        Log.WriteVerbose("Reject friendship");
                        inviter.Friends.Remove(friendProfile);
                        friendProfile.Friends.Remove(inviter);
                        ProfileStatisticsUpdater.UpdateFriends(session, inviter, friendProfile);
                        //SendMessage(friendProfile.Settings.NotificationSocial, inviter, friendProfile, data.Message, MessageType.FriendshipRejected
                        //    , "RejectFriendshipEmailSubject", "RejectFriendshipEmailMessage", inviter.UserName, DateTime.Now, data.Message);
                        NewSendMessageEx(inviter.Settings.NotificationSocial, inviter, friendProfile, "RejectFriendshipEmailSubject", "RejectFriendshipEmailMessage", inviter.UserName, DateTime.Now, data.Message);
                        //messageService.SendSystemMessage(data.Message, inviter, friendProfile, MessageType.FriendshipRejected);
                    }
                    else
                    {
                        existingInvitation = session.Get<FriendInvitation>(new FriendInvitation()
                        {
                            Inviter = inviter,
                            Invited = friendProfile
                        });
                        //user whats to cancel invitation
                        if (existingInvitation != null)
                        {
                            Log.WriteVerbose("Reject invitation");
                            session.Delete(existingInvitation);
                        }
                        else
                        {
                            throw new CannotAcceptRejectInvitationDoesntExistException("There is no invitation to reject");
                        }
                    }
                }


                tx.Commit();
                Log.WriteVerbose("Operation completed");
                if (returnValue != null)
                {
                    return ObjectsConverter.ConvertFriendInvitation(inviter, returnValue);
                }
                return null;
            }
        }
    }
}
