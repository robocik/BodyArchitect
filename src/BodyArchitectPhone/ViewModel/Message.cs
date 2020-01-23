using System;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.ViewModel;
using Coding4Fun.Phone.Controls;

namespace WP7ConversationView
{
  
  public enum MessageSide
  {
    Me,
    You
  }

  [DataContract]
  public class Message
  {
      //TODO:These three properites are temporary solution for tombstoning blogcomments
      [DataMember]
    public TrainingDayCommentDTO TrainingDayComment { get; set; }

    public MessageViewModel MessageViewModel { get; set; }

    public InvitationItemViewModel InvitationItemViewModel { get; set; }

      public ChatBubbleDirection Direction
      {
          get { return Side == MessageSide.Me ? ChatBubbleDirection.LowerRight : ChatBubbleDirection.UpperLeft; }
      }

      [DataMember]
      public UserDTO User { get; set; }
      [DataMember]
    public string Text { get; set; }

      [DataMember]
    public string UserName { get; set; }

      [DataMember]
    public DateTime Timestamp { get; set; }

      [DataMember]
    public MessageSide Side { get; set; }

      [DataMember]
    public PictureInfoDTO Picture { get; set; }
  }
}
