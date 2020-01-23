//using BodyArchitect.Client.Resources.Localization;
//using BodyArchitect.Service.V2.Model;

//namespace BodyArchitect.Client.UI.UserControls
//{
//    public class SystemMessages
//    {
//        public static string GetMessageTopic(MessageDTO message)
//        {
//            if (message.MessageType == MessageType.InvitationAccepted)
//            {
//                return SystemMessageStrings.InvitationAccepted_Topic;
//            }
//            else if (message.MessageType == MessageType.FriendshipRejected)
//            {
//                return SystemMessageStrings.FriendshipRejected_Topic;
//            }
//            if (message.MessageType == MessageType.InvitationRejected)
//            {
//                return SystemMessageStrings.InvitationRejected_Topic;
//            }
//            if(message.MessageType==MessageType.FriendProfileDeleted)
//            {
//                return SystemMessageStrings.FriendProfileDeleted_Topic;
//            }
//            if (message.MessageType == MessageType.ProfileCreated)
//            {
//                return SystemMessageStrings.ProfileCreated_Topic;
//            }
//            if (message.MessageType == MessageType.TrainingDayAdded)
//            {
//                return SystemMessageStrings.TrainingDayAdded_Topic;
//            }
//            if (message.MessageType == MessageType.BlogCommentAdded)
//            {
//                return SystemMessageStrings.BlogCommentAdded_Topic;
//            }
//            if (message.MessageType == MessageType.FavoriteProfileDeleted)
//            {
//                return SystemMessageStrings.FavoriteProfileDeleted_Topic;
//            }
//            if (message.MessageType == MessageType.WorkoutPlanVoted)
//            {
//                return SystemMessageStrings.WorkoutPlanVoted_Topic;
//            }
//            if (message.MessageType == MessageType.ExerciseVoted)
//            {
//                return SystemMessageStrings.ExerciseVoted_Topic;
//            }
//            return message.Topic;
//        }

//        public static string GetMessageContent(MessageDTO message)
//        {
//            if (message.MessageType == MessageType.InvitationAccepted)
//            {
//                return string.Format(SystemMessageStrings.InvitationAccepted_Content,message.Sender.UserName,message.Content);
//            }
//            else if (message.MessageType == MessageType.FriendshipRejected)
//            {
//                return string.Format(SystemMessageStrings.FriendshipRejected_Content, message.Sender.UserName, message.Content);
//            }
//            if (message.MessageType == MessageType.InvitationRejected)
//            {
//                return string.Format(SystemMessageStrings.InvitationRejected_Content, message.Sender.UserName, message.Content);
//            }
//            if (message.MessageType == MessageType.FriendProfileDeleted)
//            {
//                return string.Format(SystemMessageStrings.FriendProfileDeleted_Content,message.Sender.UserName);
//            }
//            if (message.MessageType == MessageType.ProfileCreated)
//            {
//                return SystemMessageStrings.ProfileCreated_Content;
//            }
//            if (message.MessageType == MessageType.TrainingDayAdded)
//            {
//                return string.Format(SystemMessageStrings.TrainingDayAdded_Content, message.Content.Split(','));
//            }
//            if (message.MessageType == MessageType.BlogCommentAdded)
//            {
//                return string.Format(SystemMessageStrings.BlogCommentAdded_Content,message.Content.Split(','));
//            }
//            if (message.MessageType == MessageType.FavoriteProfileDeleted)
//            {
//                return string.Format(SystemMessageStrings.FavoriteProfileDeleted_Content, message.Sender.UserName);
//            }

//            if (message.MessageType == MessageType.WorkoutPlanVoted)
//            {
//                return string.Format(SystemMessageStrings.WorkoutPlanVoted_Content, message.Content.Split(','));
//            }
//            if (message.MessageType == MessageType.ExerciseVoted)
//            {
//                return string.Format(SystemMessageStrings.ExerciseVoted_Content, message.Content.Split(','));
//            }
//            return message.Content;
//        }
//    }
//}
