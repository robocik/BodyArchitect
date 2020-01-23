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
    public partial class SuplementItemDTO
    {
        public SuplementItemDTO()
        {
            Time=new BATimeDTO();
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [UseSameReference]
        public BodyArchitect.Service.V2.Model.SuplementDTO Suplement
        {
            get
            {
                return this.SuplementField;
            }
            set
            {
                if ((object.ReferenceEquals(this.SuplementField, value) != true))
                {
                    this.SuplementField = value;
                    this.RaisePropertyChanged("Suplement");
                }
            }
        }
    }
}
