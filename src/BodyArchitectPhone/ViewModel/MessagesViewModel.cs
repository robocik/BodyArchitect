using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using WP7ConversationView;

namespace BodyArchitect.WP7.ViewModel
{
    public class MessagesViewModel : ViewModelBase
    {
        ObservableCollection<Message> messages = new ObservableCollection<Message>();
        ObservableCollection<Message> invitations = new ObservableCollection<Message>();

        public MessagesViewModel(ProfileInformationDTO info)
        {
            if (ApplicationState.Current.Cache.Messages.IsLoaded)
            {
                foreach (var dto in ApplicationState.Current.Cache.Messages.Items.Values)
                {
                    var viewModel = new MessageViewModel(dto);

                    var msg = new Message();
                    msg.Picture = viewModel.User.Picture;
                    msg.User = viewModel.User;
                    msg.UserName = viewModel.UserName;
                    msg.Side = viewModel.User.IsMe ? MessageSide.Me : MessageSide.You;
                    msg.Text = viewModel.Topic;
                    msg.Timestamp = viewModel.Date;
                    msg.MessageViewModel = viewModel;
                    messages.Add(msg);
                }
            }

            foreach (var dto in info.Invitations)
            {
                var viewModel = new InvitationItemViewModel(dto);
                var msg = new Message();
                msg.Picture = viewModel.User.Picture;
                msg.User = viewModel.User;
                msg.UserName = viewModel.UserName;
                msg.Side = dto.Inviter == null || dto.Inviter.IsMe ? MessageSide.Me : MessageSide.You;
                msg.Text = viewModel.OperationMessage;
                msg.Timestamp = viewModel.CreatedDateTime;
                msg.InvitationItemViewModel = viewModel;
                invitations.Add(msg);
            }
        }


        public ObservableCollection<Message> Messages
        {
            get { return messages; }
        }

        public ObservableCollection<Message> Invitations
        {
            get { return invitations; }
        }
    }
}
