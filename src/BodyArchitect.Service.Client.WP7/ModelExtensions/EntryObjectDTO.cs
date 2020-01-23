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
using BodyArchitect.Service.Client.WP7.ModelExtensions;

namespace BodyArchitect.Service.V2.Model
{
    public partial class EntryObjectDTO
    {
        public EntryObjectDTO()
        {
            ReportStatus = ReportStatus.ShowInReport;
        }
        //TODO: in TrainingDay in old version we had OriginalVersion (and Version of course). Now version is moved to EntryObject class
        //so mabye we must implement originalVersion as well
        [System.Runtime.Serialization.DataMemberAttribute()]
        [DoNotChecksum]
        public int Version
        {
            get
            {
                return this.VersionField;
            }
            set
            {
                if ((this.VersionField.Equals(value) != true))
                {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [DoNotChecksum]
        public BodyArchitect.Service.V2.Model.TrainingDayInfoDTO TrainingDay
        {
            get
            {
                return this.TrainingDayField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TrainingDayField, value) != true))
                {
                    this.TrainingDayField = value;
                    this.RaisePropertyChanged("TrainingDay");
                }
            }
        }

    }
}
