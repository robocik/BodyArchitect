using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;

namespace BodyArchitect.Service.V2.Model
{
    public partial class TrainingDayInfoDTO
    {
        public bool IsMine
        {
            get { return ProfileId == Guid.Empty || ProfileId == ApplicationState.Current.SessionData.Profile.GlobalId; }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [DoNotChecksum]
        public int CommentsCount
        {
            get
            {
                return this.CommentsCountField;
            }
            set
            {
                if ((this.CommentsCountField.Equals(value) != true))
                {
                    this.CommentsCountField = value;
                    this.RaisePropertyChanged("CommentsCount");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [DoNotChecksum]
        public System.Nullable<System.DateTime> LastCommentDate
        {
            get
            {
                return this.LastCommentDateField;
            }
            set
            {
                if ((this.LastCommentDateField.Equals(value) != true))
                {
                    this.LastCommentDateField = value;
                    this.RaisePropertyChanged("LastCommentDate");
                }
            }
        }
    }
}
